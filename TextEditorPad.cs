/* Author: Stevenson Suhardy
 * Date Created: April 4, 2022
 * Last Modified: April 4, 2022
 * App Name: Text Editor
 * App description: This app is basically a notepad now and can create, edit, save, and open .txt files, excluding some of the complex functions like, print, or find.
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class formTextEditorPad : Form
    {
        // Constant declarations
        const string defaultFileName = "Untitled.txt";
        // Constant for buttons
        const int yes = 1;
        const int no = 2;
        // Configuring the fileName to be the defaultFileName for the first time.
        string fileName = defaultFileName;
        // To keep track if anything was changed in the text
        bool isFileChanged = false;
        // To keep track if the file is saved or not
        bool isSaved = false;
        int buttonNum;

        public formTextEditorPad()
        {
            InitializeComponent();
        }
        #region "Event Handlers"
        /// <summary>
        /// This event handler will occur when the form loads. It will load the form title and clearing the clipboard
        /// </summary>
        private void TextEditorPad_Load(object sender, EventArgs e)
        {
            // Changing the form title to the current fileName, which is the defaultFileName at this point.
            this.Text = fileName;
            // Clearing clipboard to prevent any unnecessary data.
            Clipboard.Clear();
        }
        /// <summary>
        /// This event handler will occur when the user presses the "Alt + X" or the exit button in the file menu. The application will exit without any confirmation when there are no texts inside the textbox or the file has not been changed. If the textbox contains something and the file has been changed, prompt a messagebox to confirm exit without saving and open save dialog if the user does want to save.
        /// </summary>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If the textbox is empty or there are no texts
            if (textBoxPad.Text == string.Empty)
            {
                // Close the application
                Close();
            }
            // If the file is changed
            else if (isFileChanged)
            {
                // Declaring a variable to see which button the user clicked
                buttonNum = ConfirmClose(fileName); // Calling a function for the messagebox.
                // If the user clicks yes button
                if (buttonNum == yes)
                {
                    // Call the SaveAs event handler
                    SaveToolStripMenuItem_Click(sender, e);
                    // If the save button is clicked, if something else was clicked (i.e cancel or X button), then do nothing and don't exit the app.
                    if (isSaved)
                    {
                        // Close the application
                        Close();
                    }
                }
                // If the user clicks no button
                else if (buttonNum == no)
                {
                    // Close the application
                    Close();
                }
                // If the user clicks the cancel button, close the messagebox and remain in the application.
            }
            // If the file is not changed at all
            else if (!isFileChanged)
            {
                // Close the application
                Close();
            }
        }
        /// <summary>
        /// This event handler will occur when the user clicks the Select All button in the edit menu. It will select all texts inside the textbox.
        /// </summary>
        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Select all texts inside the textbox
            textBoxPad.SelectAll();
        }
        /// <summary>
        /// This event handler will occur when the user clicks the SaveAs button in the file menu. It will open a save file dialog where the user can change the name, and choose which path the user want to save it to.
        /// </summary>
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Creating a SaveFileDialog object
            SaveFileDialog saveAs = new SaveFileDialog();
            // Setting the name of the file to the current fileName, and only get the fileName without the path.
            saveAs.FileName = Path.GetFileName(fileName);
            // Filter the file extensions in the save file dialog to only .txt files.
            saveAs.Filter = "Text files (*.txt)|*.txt";
            // If the user clicks the OK button
            if (saveAs.ShowDialog() == DialogResult.OK)
            {
                // Change the current fileName to the fileName that was set by the user.
                fileName = saveAs.FileName;
                // Call the SaveFile function, which will save the file
                SaveFile(fileName);
                // Change the isSaved to true
                isSaved = true;
            }
            // If the user clicks anything else
            else
            {
                // Change the isSaved is false
                isSaved = false;
                // Show a messagebox that warns the user that their work is not saved
                MessageBox.Show("Your work has not been saved.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        /// <summary>
        /// This event handler will occur when the user presses "Ctrl + C" or when the copy button in the edit menu is clicked. This will save the current selected text into the clipboard and store it for later use.
        /// </summary>
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If the selected text is not nothing or an empty string
            if (textBoxPad.SelectedText != string.Empty)
            {
                // Declaring variable to store the selected text
                string selectedText = textBoxPad.SelectedText;
                // Storing the selected text into the clipboard
                Clipboard.SetText(selectedText);
            }
        }
        /// <summary>
        /// This event handler will occur when the user presses "Ctrl + V" or the paste button in the edit menu. This will take whatever text is inside the clipboard and put it inside the selected line of the user. 
        /// </summary>
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If there are text inside the clipboard
            if (Clipboard.GetText() != string.Empty)
            {
                if (textBoxPad.SelectedText == string.Empty)
                {
                    // Append the text into the textbox
                    textBoxPad.AppendText(Clipboard.GetText());
                }
                else
                {
                    // Changes the selected text to the copied text
                    textBoxPad.SelectedText = Clipboard.GetText();
                }
            }
        }
        /// <summary>
        /// This event handler will occur when the user presses "Ctrl + X" or the cut button in the edit menu. The cut button will copy the 
        /// </summary>
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Calling the copy event handler
            CopyToolStripMenuItem_Click(sender, e);
            // Deleting the selected text from the textbox
            textBoxPad.SelectedText = string.Empty;
        }
        /// <summary>
        /// This event handler will occur when the user clicks the open button in the file menu. It will open a open file dialog and filters the files to .txt files. The user can select any text files and then after selecting a text file and clicking OK, the text file will be read and written into the textbox.
        /// </summary>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If the file is changed
            if (isFileChanged)
            {
                // Declaring a buttonNum to call the ConfirmClose() functions.
                buttonNum = ConfirmClose(fileName);
                // If the user clicks yes
                if (buttonNum == yes)
                {
                    // Call the save as event
                    SaveToolStripMenuItem_Click(sender, e);
                    // If the user clicks the save button
                    if (isSaved)
                    {
                        // Call the open file function
                        OpenFile();
                    }
                }
                // If the user clicks no
                else if (buttonNum == no)
                {
                    // Call the open file function
                    OpenFile();
                }
            }
            // If the file is not changed
            else if (!isFileChanged)
            {
                // Call the open file function
                OpenFile();
            }
        }
        /// <summary>
        /// This event handler will occur when the user presses "Ctrl + N" or the new button in the file menu. This event will reset the application to it's default form, meaning creating a brand new file.
        /// </summary>
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If the file is changed
            if (isFileChanged)
            {
                // Calling the ConfirmClose() function
                buttonNum = ConfirmClose(fileName);
                // If the user clicks yes
                if (buttonNum == yes)
                {
                    // Call the save as event
                    SaveToolStripMenuItem_Click(sender, e);
                    // If the user saves the file
                    if (isSaved)
                    {
                        // Call the NewFile() function
                        NewFile();
                    }
                }
                // If the user clicks no
                else if (buttonNum == no)
                {
                    // Call the NewFile() function
                    NewFile();
                }
            }
            // If the file is not changed
            else if (!isFileChanged)
            {
                // Call the NewFile() function
                NewFile();
            }
        }
        /// <summary>
        /// This event handler will occur when the user presses the "Ctrl + S" or save button in the file menu. This will save the file instantly if it was saved before. However, when the file has never been saved, call the save as event handler instead.
        /// </summary>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If the fileName is still default
            if (fileName == defaultFileName)
            {
                // Call the save as event
                SaveAsToolStripMenuItem_Click(sender, e);
            }
            // Anything else
            else
            {
                // Call the save file function
                SaveFile(fileName);
            }
        }
        /// <summary>
        /// This event handler will occur when the user presses the about button in the help menu. It will show a messagebox about this app.
        /// </summary>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Display the following messagebox
            MessageBox.Show("NETD-2202\n\nLab #5\nThis is a text editor app made very similar to notepad as a reference. This app is made for Lab 5 in the NETD-2202 course in Durham College, Oshawa, Canada\n\nStevenson Suhardy", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// This event handler will occur when there are changes in the textbox.
        /// </summary>
        private void TextBoxPad_TextChanged(object sender, EventArgs e)
        {
            // Changing the isFileChanged to true
            isFileChanged = true;
            // If the form title does not contain * in it
            if (this.Text != "*" + Path.GetFileName(fileName))
            {
                // Put a star in it to indicate that the file is changed and has not been saved
                this.Text = "*" + Path.GetFileName(fileName);
            }
        }
#endregion
        #region "Functions / Methods"
        /// <summary>
        /// This function takes 1 parameter, and it will open file stream and using stream writer to write everything from the textbox into the file and then save it.
        /// </summary>
        /// <param name="theFileName"></param>
        private void SaveFile(string theFileName)
        {
            // Creating a new FileStream object
            FileStream fileToAccess = new FileStream(theFileName, FileMode.Create, FileAccess.Write);
            // Creating a StreamWriter object
            StreamWriter writer = new StreamWriter(fileToAccess);
            // Using streamwriter to write every text in the textbox into the file
            writer.Write(textBoxPad.Text);
            // Close the writer to release the resources
            writer.Close();
            // Change the form title to the current file name
            this.Text = Path.GetFileName(theFileName);
            // Changing isFileChanged to false
            isFileChanged = false;
        }
        /// <summary>
        /// This function will open a confirmation messagebox if the file has not been saved. It returns an integer value to see what button the user clicked.
        /// </summary>
        /// <param name="theFileName"></param>
        /// <returns>buttonResult</returns>
        private int ConfirmClose(string theFileName)
        {
            // Declaring variable
            int buttonResult = 0;
            // Creating a DialogResult object
            DialogResult result;
            // If the fileName is default
            if (theFileName == defaultFileName)
            {
                // Show the following messagebox
                result = MessageBox.Show("Do you want to save your file before closing?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            }
            // Anything else
            else
            {
                // Display the following messagebox
                result = MessageBox.Show("Do you want to save your file to " + theFileName + " before closing?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            }
            // If the user clicks yes
            if (result == DialogResult.Yes)
            {
                // Return 1
                buttonResult = 1;
            }
            // If the user clicks no
            else if (result == DialogResult.No)
            {
                // Return 2
                buttonResult = 2;
            }
            // If the user clicks cancel, close the messagebox and don't do anything
            // Return buttonResult
            return buttonResult;
        }
        /// <summary>
        /// This function will open a OpenFileDialog and filter the files to only .txt files. This will read the selected file, and write into the textbox, also transfer the file name to the form title.
        /// </summary>
        private void OpenFile()
        {
            // Create an OpenFileDialog object
            OpenFileDialog openDialog = new OpenFileDialog();
            // Creating the filter for the dialog to .txt files
            openDialog.Filter = "Text files (*.txt)|*.txt";

            // If the user clicks OK
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                // Create a FileStream object.
                FileStream fileToAccess = new FileStream(openDialog.FileName, FileMode.Open, FileAccess.Read);
                // Create a StreamReader object.
                StreamReader reader = new StreamReader(fileToAccess);

                // Clearing the textBoxPad
                textBoxPad.Clear();

                // Read until the end of the file
                while (!reader.EndOfStream)
                {
                    // Declaring a new variable and setting it to a line in the file
                    string newText = reader.ReadLine();
                    // Entering the text to the textbox, and adding a new line because ReadLine reads until the end of the line
                    textBoxPad.AppendText(newText + "\r\n");
                }
                // Close the reader
                reader.Close();
                // Changing fileName to the opened file name
                fileName = openDialog.FileName;
                // Changing the form title to the opened file name without the path
                this.Text = Path.GetFileName(fileName);
                // Changing isFileChanged to false because user just opened it
                isFileChanged = false;
            }
        }
        /// <summary>
        /// This function will reset the app to it's default state without anything in it, but does not clear the clipboard just in case user wants to paste inside a new file.
        /// </summary>
        private void NewFile()
        {
            // Changing the fileName back to default
            fileName = defaultFileName;
            // Clearing the textbox
            textBoxPad.Clear();
            // Changing the form title to the fileName
            this.Text = fileName;
            // Changing isFileChanged to false
            isFileChanged = false;
            // Changing isSaved to false
            isSaved = false;
        }
        #endregion
    }
}