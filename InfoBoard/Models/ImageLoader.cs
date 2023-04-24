using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoBoard.Models;
internal class ImageLoader
{
    public ObservableCollection<string> ImageBasket { get; set; } = new ObservableCollection<string>();

    public ImageLoader() =>
        LoadNotes();

    public void LoadNotes()
    {
        ImageBasket.Clear();

        // Get the folder where the notes are stored.
        string appDataPath = FileSystem.AppDataDirectory;

        // Use Linq extensions to load the *.notes.txt files.
        IEnumerable<string> imageList = Directory

                                    // Select the file names from the directory
                                    .EnumerateFiles(appDataPath, "*.jpg")
                                    
                                    // With the final collection of notes, order them by date
                                    .OrderBy(note => note);

        // Add each note into the ObservableCollection
        foreach (string note in imageList)
            ImageBasket.Add(note);
    }
}