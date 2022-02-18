using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Text;
namespace IntroMeta
{
    public class Program
    {
            static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();

            static void Main(string [] args)
            {
                DirectoryInfo di = new DirectoryInfo(@"c:\MetaPics");
                int counter = 0;               
                WalkDirectory(di, counter);
            }
             
             // function: adding comments
            static void addImageComment(string filePath, string comments){
                BitmapDecoder decoder = null;
                BitmapFrame bitmapFrame = null;
                BitmapMetadata metadata = null;
                FileInfo originalImage = new FileInfo(filePath);

                if(File.Exists(filePath)){
                   //load the jpg file with a JpegBitmapDecoder 
                   System.Console.WriteLine("File path exists" + filePath);
                    using (Stream jpegStreamIn = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None) ){
                        decoder = new JpegBitmapDecoder(jpegStreamIn, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                    }

                    bitmapFrame = decoder.Frames[0];
                    metadata = (BitmapMetadata ) bitmapFrame.Metadata;

                    if(bitmapFrame != null){
                        BitmapMetadata metaData = (BitmapMetadata) bitmapFrame.Metadata.Clone();

                        if(metaData != null){
                            //modify the metadata
                            metaData.Comment = comments;
                            //get an encoder to create a new jpg file with the new data                        
                            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(bitmapFrame, bitmapFrame.Thumbnail, metaData, bitmapFrame.ColorContexts));
                            originalImage.Delete();
                            // save the new image.
                             using (Stream jpegStreamOut = File.Open(filePath, FileMode.CreateNew, FileAccess.ReadWrite)){   
                                encoder.Save(jpegStreamOut);
                            }   

                        }
                    }

                 }
            }
            //funtion to walk through all the directories
    static void WalkDirectory(System.IO.DirectoryInfo root, int counter){
        System.IO.FileInfo[] files = null;
        System.IO.DirectoryInfo[] subDirs = null;
        try {
            files = root.GetFiles("*");

        }
        catch (UnauthorizedAccessException e){
            log.Add(e.Message);

        }
        catch (System.IO.DirectoryNotFoundException e){
            Console.WriteLine(e.Message);
        }
        string textName = @"c:\Text\" + "text" +counter + ".txt";
        List <string> list = new List<string>();

        if(files != null){
            foreach (System.IO.FileInfo fi in files){
                string fileName = fi.Name.ToString();
                string path = root.ToString() + @"\" + fileName;
                DateTime date = File.GetLastWriteTime(path);
                string comment = "Batch!!" + counter.ToString()+ " " + date.ToString();
                string commentText = "Batch!!" + counter.ToString()+ " " + fileName + " "+ date.ToString();    
                list.Add(commentText); 
                addImageComment(path, comment);
            }
            writeTextFile(textName, list);               
        }
        subDirs = root.GetDirectories();
        foreach(System.IO.DirectoryInfo dirInfo in subDirs){
            counter++;
            WalkDirectory(dirInfo, counter);
        }
    }

//function writing to the text files.
static void writeTextFile(string textLine, List<string> list){
    try {
        StreamWriter sw = new StreamWriter(textLine);
            for(int i = 0; i < list.Count; i++){
                sw.Write(list[i]);
                sw.Write("\n");

            }
                sw.Close();
         }
    catch(Exception e){
         Console.WriteLine("Exception:" + e.Message);
     }

}

    }

}