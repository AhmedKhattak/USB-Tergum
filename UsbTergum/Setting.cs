using System;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace UsbTergum
{
    
     class Setting
    {


         /// <summary>
         /// gets the refrences from form.cs and sets their values
         /// </summary>
         /// <param name="shouldLog"></param>
         /// <param name="shouldDrillFolders"></param>
         /// <param name="shouldOverwriteFiles"></param>
         /// <param name="backupFolderPath"></param>
         /// <param name="fileTypes"></param>
         public static void LoadSettings( ref bool shouldLog, ref bool shouldDrillFolders,
             ref bool shouldOverwriteFiles, ref string backupFolderPath, ref List<string> fileTypes)
        {
            
            XmlReader xmlReader=null;
            try
            {
                xmlReader = XmlReader.Create("settings.xml"); 
                
                //clear the file types list before refilling it
                fileTypes.Clear();
                while (xmlReader.Read())
                {
                    if ((xmlReader.NodeType == XmlNodeType.Element))
                    {
                        if (xmlReader.HasAttributes)
                        {
                            switch(xmlReader.Name)
                            {
                             
                                case "shouldLog":
                                    if (xmlReader.GetAttribute(0) == "True")
                                    { shouldLog = true; }
                                    else if (xmlReader.GetAttribute(0) == "False")
                                    { shouldLog = false; }
                                    else
                                    { shouldLog = false; }
                                    break;
                                case "shouldDrillFolders":
                                    if (xmlReader.GetAttribute(0) == "True")
                                    { shouldDrillFolders = true; }
                                    else if (xmlReader.GetAttribute(0) == "False")
                                    { shouldDrillFolders= false; }
                                    else
                                    { shouldDrillFolders= false; }
                                    break;
                                case "shouldOverwriteFiles":
                                    if (xmlReader.GetAttribute(0) == "True")
                                    { shouldOverwriteFiles = true; }
                                    else if (xmlReader.GetAttribute(0) == "False")
                                    { shouldOverwriteFiles = false; }
                                    else
                                    { shouldOverwriteFiles = false; }
                                    break;
                                case "backupFolderPath":
                                    backupFolderPath = xmlReader.GetAttribute(0);
                                    break;
                                case "fileTypes":
                                    fileTypes.Add(xmlReader.GetAttribute(0));
                                    break;

                            }
                        
                        }
                            

                    }
                }
            }
            catch(Exception x)
            {
                MessageBox.Show(x.ToString(), "Fatal Error",MessageBoxButtons.OK ,MessageBoxIcon.Error);
            }
            finally
            {
                //always close the reader no matter what 
                if(xmlReader!=null)
                xmlReader.Close();

            }
        }


       /// <summary>
       /// gets the refrences from form.cs and gets their values and saves them in xml file 
       /// </summary>
       /// <param name="shouldLog"></param>
       /// <param name="shouldDrillFolders"></param>
       /// <param name="shouldOverwriteFiles"></param>
       /// <param name="backupFolderPath"></param>
       /// <param name="fileTypes"></param>
        public static void SaveSettings( ref bool shouldLog, ref bool shouldDrillFolders,
            ref bool shouldOverwriteFiles, ref string backupFolderPath, ref List<string> fileTypes)
        {
            XmlWriter xmlWriter=null;  
            try
            {
                
                xmlWriter = XmlWriter.Create("settings.xml"); 
               
                xmlWriter.WriteStartDocument();
                //settings ie the root node
                xmlWriter.WriteStartElement("settings");

                //logging
                xmlWriter.WriteStartElement("shouldLog");
                xmlWriter.WriteAttributeString("bool", shouldLog.ToString());
                xmlWriter.WriteEndElement();

                //search nested folders
                xmlWriter.WriteStartElement("shouldDrillFolders");
                xmlWriter.WriteAttributeString("bool", shouldDrillFolders.ToString());
                xmlWriter.WriteEndElement();

                //overwrite files
                xmlWriter.WriteStartElement("shouldOverwriteFiles");
                xmlWriter.WriteAttributeString("bool", shouldOverwriteFiles.ToString());
                xmlWriter.WriteEndElement();

                //backup folder path
                xmlWriter.WriteStartElement("backupFolderPath");
                xmlWriter.WriteAttributeString("string",backupFolderPath.ToString());
                xmlWriter.WriteEndElement();

                //file types
                //loop through array and save filetypes


                if (fileTypes.Count > 0)
                {
                    for (int x = 0; x < fileTypes.Count; x++)
                    {
                        xmlWriter.WriteStartElement("fileTypes");
                        xmlWriter.WriteAttributeString("string", fileTypes[x]);
                        xmlWriter.WriteEndElement();
                    }
                }
               


                //write end of document
                xmlWriter.WriteEndDocument();
                //close the writer
                xmlWriter.Close();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //always close the xml writer no matter what
                if (xmlWriter != null)
                    xmlWriter.Close();

            }
        }
    }
}
