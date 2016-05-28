using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Dolinay;
using System.Diagnostics;
using System.IO;
using System.Threading;
//using System.Linq;

namespace UsbTergum
{
    public partial class Form1 : Form
    {


      
        private bool shouldLog;
        private bool shouldDrillFolders;
        private bool shouldOverwriteFiles;
        private bool isWatching;
        private string backupFolderPath = "";
        private List<string> fileTypes;
        private string driveLetter="none";
        private DriveInfo usbInfo;
        private DriveDetector driveDetector = null;
        private FolderBrowserDialog folderBrowserDialog1;
        private ToolTip tooltip;

        /// <summary>
        /// public getters
        /// </summary>
        public bool shouldlog { get { return shouldLog; } }
        public bool shoulddrillfolders { get { return shouldDrillFolders; } }
        public bool shouldoverwritefiles { get { return shouldOverwriteFiles; } }
        public string backupfolderpath { get { return backupFolderPath; } }
        public List<string> filetypes { get { return fileTypes; } }
        public string driveletter { get { return driveLetter; } } 
       


        public Form1()
        {
            InitializeComponent();
            //set notifyicon context menu
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

          
          
            //set tooltips
            tooltip = new ToolTip();
           
            tooltip.SetToolTip(Main_heading, "USB Backup ! backup your usb with ease !");

            fileTypes = new List<string>();

            Setting.LoadSettings(ref shouldLog, ref shouldDrillFolders, ref shouldOverwriteFiles,
               ref backupFolderPath, ref fileTypes);
            //when function does its thing then  set the gui elements 

            tooltip.SetToolTip(FilePath, backupFolderPath);

            if (shouldOverwriteFiles ==true)
            { overwriteRadioYes.Checked = true; }
            else if (shouldOverwriteFiles == false)
            { overwriteRadioNo.Checked = true; }


        


            if (shouldLog == true)
            { createLogRadioYes.Checked = true; }
            else if (shouldLog ==false)
            {  createLogRadioNo.Checked = true; }


            if (shouldDrillFolders == true)
            { searchsubRadioYes.Checked = true;  }
            else if (shouldDrillFolders == false )
            { searchsubRadioNo.Checked = true; }

            if(backupFolderPath.Length>0)
            {
            if (backupFolderPath.Length > 34)
            { FilePath.Text = backupFolderPath.Substring(0, 30) + "..."; }
            else
            { FilePath.Text = backupFolderPath; }
            }

           foreach(string file in fileTypes)
           {
               switch(file)
               {
                   case "pdf":
                       pdfBox.Checked = true;
                       break;
                   case "ppt":
                       pptBox.Checked = true;
                       break;
                   case "txt":
                       txtBox.Checked = true;
                       break;
                   case "doc":
                       docBox.Checked = true;
                       break;
                   case "xls":
                       xlsBox.Checked = true;
                       break;
                   case "vbs":
                       vbsBox.Checked = true;
                       break;
                   case "exe":
                       exeBox.Checked = true;
                       break;
                   case "html":
                       htmlBox.Checked = true;
                       break;
                   case "css":
                       cssBox.Checked = true;
                       break;
                   case "py":
                       pyBox.Checked = true;
                       break;
                   case "cpp":
                       cppBox.Checked = true;
                       break;
                   case "*":
                       allBox.Checked = true;
                       break;
                   case "gif":
                       gifBox.Checked = true;
                       break;
                   case "png":
                       pngBox.Checked = true;
                       break;
                   case "jpg":
                       jpgBox.Checked = true;
                       break;
                       


               }
               
                   
               
              

           }
           
            
            
            //instantiate a drive detection object
            driveDetector = new DriveDetector();
            //subscribe to drive arrival event
            driveDetector.DeviceArrived += new DriveDetectorEventHandler(OnDriveArrived);
            isWatching = true;
           







        }

        private void setDriveLetter(string dletter)
        {
            driveLetter = dletter;
        }


        //drive arrival event handler
        private void OnDriveArrived(object sender, DriveDetectorEventArgs e)
        {
                usbInfo = new DriveInfo(e.Drive.ToString().Trim('\\'));
                FileCopyDisplay form = new FileCopyDisplay(e.Drive.ToString().Trim('\\'));
                form.Show();
          
            
        }


      
        /// <summary>
        /// opens the backup folder by creating a new process and passing the folder path as an argument to the explorer.exe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("explorer.exe",backupFolderPath);
            }
            catch(Exception z)
            {
                MessageBox.Show(z.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {

                notifyIcon1.Icon = new Icon("usb_stick.ico");
                notifyIcon1.Visible = true;
                notifyIcon1.Text = "UsbTergum";
                notifyIcon1.ShowBalloonTip(3 * 1000, "UsbTergum", "UsbTergum has started watching for usb device arrivals", ToolTipIcon.Info);
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       


     


        /// <summary>
        /// select folder path for backup folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        
        {
            try
            {
                /// display a dialog to select the folder to hold the backup files
                folderBrowserDialog1 = new FolderBrowserDialog();
                folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
                folderBrowserDialog1.SelectedPath = Environment.SpecialFolder.Desktop.ToString();
                DialogResult = folderBrowserDialog1.ShowDialog();
                //if the result is ok then set the path as the backup
                if (DialogResult == DialogResult.OK)
                {
                   
                    backupFolderPath = folderBrowserDialog1.SelectedPath;
                    //set tooltip
                    tooltip.SetToolTip(FilePath, backupFolderPath);
                    if (backupFolderPath.Length > 34)
                    { FilePath.Text = backupFolderPath.Substring(0, 30)+"..."; }
                    else
                    { FilePath.Text = backupFolderPath; }
                    
                }
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

     
            
        }


        /// <summary>
        /// Apply settings 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplySettings_Click(object sender, EventArgs e)
        {
            
            fileTypes.Clear();
            foreach (CheckBox gb in groupBox1.Controls)
            {
               switch(gb.Name)
               {

                   case "xlsBox":
                     if(gb.CheckState.ToString()=="Checked")
                     {
                         fileTypes.Add(gb.Text);
                     }
                       break;
                   case "pngBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "jpgBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "gifBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "allBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "pdfBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "txtBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "pptBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "csBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "docBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "exeBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "vbsBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                          fileTypes.Add(gb.Text);
                       }
                       break;
                   case "htmlBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "cssBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "pyBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;
                   case "cppBox":
                       if (gb.CheckState.ToString() == "Checked")
                       {
                           fileTypes.Add(gb.Text);
                       }
                       break;


                    


                   
               }

            }
           
           
            //check radio buttons

            if(overwriteRadioYes.Checked==true)
            { shouldOverwriteFiles = true; }
            else if (overwriteRadioNo.Checked == true)
            { shouldOverwriteFiles = false; }


            if (createLogRadioYes.Checked == true)
            { shouldLog = true;  }
            else if(createLogRadioNo.Checked==true)
            { shouldLog = false; }


            if (searchsubRadioYes.Checked == true)
            { shouldDrillFolders = true; }
            else if(searchsubRadioNo.Checked==true)
            { shouldDrillFolders = false; }


            
          Setting.SaveSettings( ref shouldLog, ref shouldDrillFolders, ref shouldOverwriteFiles,
                ref backupFolderPath , ref fileTypes);

          notifyIcon1.ShowBalloonTip(1000, "UsbTergum", "Settings applied", ToolTipIcon.Info);
         
         
        }

        private void Save_MouseEnter(object sender, EventArgs e)
        {
            Save.BackColor = Color.FromArgb(255, 0, 135, 222);
        }

        private void Save_MouseHover(object sender, EventArgs e)
        {
            Save.BackColor = Color.FromArgb(255, 0, 135, 222);
        }

        private void Save_MouseLeave(object sender, EventArgs e)
        {
            Save.BackColor = Color.FromArgb(255, 51, 153, 255);
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //change the window settings 
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
                //then show it
                this.Show();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isWatching == true)
                {
                    driveDetector.DeviceArrived -= OnDriveArrived;
                    notifyIcon1.ShowBalloonTip(3000, "UsbTergum", "Paused watching for usb arrival", ToolTipIcon.Info);
                    isWatching = false;
                    pauseToolStripMenuItem.Enabled = false;
                    pauseToolStripMenuItem.BackColor = Color.Orange;
                    pauseToolStripMenuItem.ForeColor = Color.Black;
                    resumeToolStripMenuItem.Enabled = true;
                    resumeToolStripMenuItem.BackColor = Color.White;
                    button3.Image = Properties.Resources.rsz_ios7_play_128;

                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void resumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isWatching == false)
                {
                    driveDetector.DeviceArrived += new DriveDetectorEventHandler(OnDriveArrived);
                    notifyIcon1.ShowBalloonTip(3000, "UsbTergum", "Resumed watching for usb arrival", ToolTipIcon.Info);
                    isWatching = true;
                    resumeToolStripMenuItem.Enabled = false;
                    resumeToolStripMenuItem.BackColor = Color.Blue;
                    resumeToolStripMenuItem.ForeColor = Color.White;
                    pauseToolStripMenuItem.Enabled = true;
                    pauseToolStripMenuItem.BackColor = Color.White;
                    button3.Image = Properties.Resources.rsz_2media_pause;
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// resets all the settings to default
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_Click(object sender, EventArgs e)
        {
            //remove all file types from list
            fileTypes.Clear();
            foreach (CheckBox gb in groupBox1.Controls)
            {
                switch (gb.Name)
                {

                    case "xlsBox":
                        gb.CheckState = CheckState.Unchecked;
                       
                        break;
                    case "pngBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "jpgBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "gifBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "allBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "pdfBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "txtBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "pptBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "csBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "docBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "exeBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "vbsBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "htmlBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "cssBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "pyBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;
                    case "cppBox":
                        gb.CheckState = CheckState.Unchecked;
                        break;






                }

            }


          

            overwriteRadioNo.Checked = true;
            shouldOverwriteFiles = false;
            createLogRadioNo.Checked = true;
            shouldLog = false;
            searchsubRadioNo.Checked = true;
            shouldDrillFolders = false; 
          
            Setting.SaveSettings(ref shouldLog, ref shouldDrillFolders, ref shouldOverwriteFiles,
                  ref backupFolderPath, ref fileTypes);
            notifyIcon1.ShowBalloonTip(1000, "UsbTergum", "Settings Reset", ToolTipIcon.Info);
        }


        private void Hide_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        /// <summary>
        /// controls the state of the usb device watcher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (isWatching == false)
                {
                    driveDetector.DeviceArrived += new DriveDetectorEventHandler(OnDriveArrived); //subcribe to event
                    notifyIcon1.ShowBalloonTip(1000, "UsbTergum", "Resumed watching for usb arrival", ToolTipIcon.Info);
                    isWatching = true;
                    resumeToolStripMenuItem.Enabled = false;
                    resumeToolStripMenuItem.BackColor = Color.Blue;
                    resumeToolStripMenuItem.ForeColor = Color.White;
                    pauseToolStripMenuItem.Enabled = true;
                    pauseToolStripMenuItem.BackColor = Color.White;
                    button3.Image = Properties.Resources.rsz_2media_pause;
                }
                else if(isWatching==true)
                {
                    driveDetector.DeviceArrived -= OnDriveArrived; //un subcribe from event
                    notifyIcon1.ShowBalloonTip(1000, "UsbTergum", "Paused watching for usb arrival", ToolTipIcon.Info);
                    isWatching = false;
                    pauseToolStripMenuItem.Enabled = false;
                    pauseToolStripMenuItem.BackColor = Color.Orange;
                    pauseToolStripMenuItem.ForeColor = Color.Black;
                    resumeToolStripMenuItem.Enabled = true;
                    resumeToolStripMenuItem.BackColor = Color.White;
                    button3.Image = Properties.Resources.rsz_ios7_play_128;
              
                }



                
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Main_heading_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by"+Environment.NewLine+"Adnan Jameel"+Environment.NewLine+"M.Sufian"+Environment.NewLine+"Ahmed Rafiullah","UsbTergum",MessageBoxButtons.OK,MessageBoxIcon.Question);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                //change the window settings 
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
                //then show it
                this.Show();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      
      
    }
}
