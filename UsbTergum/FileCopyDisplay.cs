using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace UsbTergum
{
    public partial class FileCopyDisplay : Form
    {
        
        public  bool shouldLog;
        public  bool shouldDrillFolders;
        public  bool shouldOverwriteFiles;
        public  string backupFolderPath = "";
        public  List<string> fileTypes;
        public string drive;
        BackgroundWorker bw;
        
       
        //could not set the drive letter using its property so used a constructer initializer instead
        public FileCopyDisplay(string driveletter)
        {
            drive = driveletter;
            InitializeComponent();
           
        }

        private void PlaceFormBottomRight()
        {
            //gets the first screen
            Screen rightmost = Screen.AllScreens[0];
            //loops through all the screens and compares which is the rightmost screen 
            foreach (Screen screen in Screen.AllScreens)
            {

                if (screen.WorkingArea.Right > rightmost.WorkingArea.Right)
                    rightmost = screen; //sets the right most screen
            }
            //sets the form postion according the rightmost screen dimensions 
            this.Left = rightmost.WorkingArea.Right - this.Width;
            this.Top = rightmost.WorkingArea.Bottom - this.Height;
        }

        /// <summary>
        /// overrides the onload event 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            PlaceFormBottomRight();
            base.OnLoad(e);
        }
        /// <summary>
        /// the on load form event will immediately start copying the files and display the progress in the progress bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileCopyDisplay_Load(object sender, EventArgs e)
        {

            Form1 data = new Form1();

            shouldLog = data.shouldlog;
            shouldDrillFolders = data.shoulddrillfolders;
            shouldOverwriteFiles = data.shouldoverwritefiles;

            backupFolderPath = data.backupfolderpath;
            fileTypes = data.filetypes;
            DriveInfo driveinformation = new DriveInfo(drive);
            this.Text = driveinformation.VolumeLabel;
            if (shouldLog == true)
            {
                try
                {
                    File.AppendAllText("LOG.txt", "File Copy started at " + DateTime.Now + "for usb : " + drive + Environment.NewLine);
                    File.AppendAllText("LOG.txt", "==================================================================================================" + Environment.NewLine);
                }
                catch(Exception x)
                {
                    //do nothing
                }
            }
            // create a worker
            bw = new BackgroundWorker();
            //allow canecllation and report progress
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            //create the event handlers
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
           
           
            //execute the the worker if srive is ready 
            if (driveinformation.IsReady)
            { bw.RunWorkerAsync(); }
           
          
           
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }


        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            progressBar1.PerformStep();
            var x = (reportProgressCustomObject)e.UserState;



            if (x.currentfile.Length > 37)
            { label2.Text = x.currentfile.Substring(0, 36) + "........."; }
            else
            { label2.Text = x.currentfile; }
            label3.Text = x.filesleft + "/" + x.totalfiles;
            progressBar1.Maximum = Convert.ToInt32( x.totalfiles);
   
            

        }


        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //cast sender as a background worker
          BackgroundWorker worker = sender as BackgroundWorker;
          try
          {
              int lenght=0;
              int filesCount = 0;
                  //get total num of files
                  foreach (string file in fileTypes)
                  {
                      foreach (string fileName in FileEnumerator.EnumerateFiles(drive, "*." + file, (shouldDrillFolders) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                      { lenght++; }

                  }
                  foreach (string file in fileTypes)
                  {
                     
                     
                      
                          foreach (string fileName in FileEnumerator.EnumerateFiles(drive, "*." + file, (shouldDrillFolders) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                          {
                              if (worker.CancellationPending == true)
                              { break; }
                              filesCount++;
                             
                              //delay loop for 1 millisecond otherwise the program crashes
                              Thread.Sleep(1);
                              try
                              {
                                  //if overwrite is false then we need to check if file of same name exists if does then append a file num until no file with the same name is found else if ovewrite is true then just copy it without checking anything
                                  if (shouldOverwriteFiles == false)
                                  {


                                      //check if file exists
                                      if (File.Exists(backupFolderPath + @"\" + fileName.Substring(fileName.LastIndexOf(@"\") + 1)))
                                      {

                                          string z = GetUniqueFilePath(backupFolderPath + @"\" + fileName.Substring(fileName.LastIndexOf(@"\") + 1));
                                          File.Copy(fileName, z);

                                      }
                                      //file of same name does not exist then copy
                                      else
                                      {

                                          File.Copy(fileName, backupFolderPath + @"\" + fileName.Substring(fileName.LastIndexOf(@"\") + 1), shouldOverwriteFiles);
                                          if (shouldLog == true)
                                          {
                                              try
                                              {
                                                  File.AppendAllText("LOG.txt", fileName + " copied to : " + backupFolderPath + " at : " + DateTime.Now + Environment.NewLine);
                                              }
                                              catch (Exception x)
                                              {

                                              }
                                          }

                                      }
                                  }
                                  else
                                  {

                                      File.Copy(fileName, backupFolderPath + @"\" + fileName.Substring(fileName.LastIndexOf(@"\") + 1), shouldOverwriteFiles);
                                      if (shouldLog == true)
                                      {
                                          try
                                          {
                                              File.AppendAllText("LOG.txt", fileName + " copied to : " + backupFolderPath + " at : " + DateTime.Now + Environment.NewLine);

                                          }
                                          catch (Exception x)
                                          {

                                          }
                                      }
                                  }
                              }
                              catch (UnauthorizedAccessException noaccess)
                              {
                                  if (shouldLog == true)
                                  {
                                      try
                                      {
                                          File.AppendAllText("LOG.txt", noaccess.Message + " : " + fileName + Environment.NewLine);
                                      }
                                      catch (Exception x)
                                      {

                                      }
                                  }
                              }
                              catch (ArgumentException arg)
                              {
                                  if (shouldLog == true)
                                  {
                                      try
                                      {
                                          File.AppendAllText("LOG.txt", arg.Message + " : " + fileName + Environment.NewLine);
                                      }
                                      catch (Exception x)
                                      {

                                      }
                                  }
                              }
                              catch (PathTooLongException plongex)
                              {
                                  if (shouldLog == true)
                                  {
                                      try
                                      {
                                          File.AppendAllText("LOG.txt", plongex.Message + " : " + fileName + Environment.NewLine);
                                      }
                                      catch (Exception x)
                                      {

                                      }
                                  }

                              }
                              catch (DirectoryNotFoundException dnotfoundex)
                              {
                                  if (shouldLog == true)
                                  {
                                      try
                                      {
                                          File.AppendAllText("LOG.txt", dnotfoundex.Message + " : " + fileName + Environment.NewLine);
                                      }
                                      catch (Exception x)
                                      {
                                      }
                                  }

                              }
                              catch (FileNotFoundException fnotfoundex)
                              {
                                  if (shouldLog == true)
                                  {
                                      try
                                      {
                                          File.AppendAllText("LOG.txt", fnotfoundex.Message + " : " + fileName + Environment.NewLine);
                                      }
                                      catch (Exception x)
                                      {

                                      }
                                  }

                              }
                              catch (IOException ioex)
                              {
                                  if (shouldLog == true)
                                  {
                                      try
                                      {
                                          File.AppendAllText("LOG.txt", ioex.Message + " : " + fileName + Environment.NewLine);
                                      }
                                      catch (Exception x)
                                      { }
                                  }
                              }
                              catch (NotSupportedException nsopex)
                              {
                                  if (shouldLog == true)
                                  {
                                      try
                                      {
                                          File.AppendAllText("LOG.txt", nsopex.Message + " : " + fileName + Environment.NewLine);
                                      }
                                      catch (Exception x)
                                      { }
                                  }

                              }
                              catch (Exception x)
                              {
                                  if (shouldLog == true)
                                  {
                                      try
                                      {
                                          File.AppendAllText("LOG.txt", x.Message + " : " + fileName + Environment.NewLine);
                                      }
                                      catch(Exception xx)
                                      { }
                                  }

                              }


                              worker.ReportProgress(filesCount, new reportProgressCustomObject(fileName, (filesCount).ToString(), lenght.ToString()));

                          }
                      }
                  
             
             
              
          }
          catch(Exception x)
          {
              MessageBox.Show(x.ToString());
          }
         
               
              
                
            
            

        }


         //<summary>
         //created a seperate class to get a custom object from the background report progress changed  
         //</summary>
        class reportProgressCustomObject
        {

            public string currentfile { get; set; }
            public string filesleft   { get; set; }
            public string totalfiles  { get; set; }
            public reportProgressCustomObject(string currentfile,string filesleft,string totalfiles)
            {

                this.currentfile = currentfile;
                this.filesleft = filesleft;
                this.totalfiles = totalfiles;
            }


        }

        /// taken from http://stackoverflow.com/questions/13049732/automatically-rename-a-file-if-it-already-exists-in-windows-way
        /// this answer http://stackoverflow.com/a/22373595
        /// <summary>
        /// gets a unique file name if file with same name already exists
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string GetUniqueFilePath(string filepath)
        {
            if (File.Exists(filepath))
            {
                string folder = Path.GetDirectoryName(filepath);
                string filename = Path.GetFileNameWithoutExtension(filepath);
                string extension = Path.GetExtension(filepath);
                int number = 0;

                //create a regex match var
                Match regex = Regex.Match(filepath, @"(.+) \((\d+)\)\.\w+");

                if (regex.Success)
                {
                    //get the filename
                    filename = regex.Groups[1].Value;
                    //get the number
                    number = int.Parse(regex.Groups[2].Value);
                }

                do
                {
                    //increment number
                    number++;
                    //create new filepath
                    filepath = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, number, extension));
                    //do this until new filepath does not exist in backup folder
                }
                while (File.Exists(filepath));
            }
            //get the new filepath
            return filepath;
        }


      



      
        //if form is closing then cancel the background worker
        private void FileCopyDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            bw.CancelAsync();
        }
    }
}
