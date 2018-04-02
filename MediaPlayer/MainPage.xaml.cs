using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MediaPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        
       
        List<MediaFileInfo> mediaFileInfos = new List<MediaFileInfo>();

        int mediaIndex;
        private MediaFileInfo mediaFilePlaying = null;
        
        public MainPage()
        {
            this.InitializeComponent();
        }
        //打开filePicker,读入文件到 mediaFileInfos;
        private async void FileButton_ClickAsy(object sender, RoutedEventArgs e)
        {

            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            fileOpenPicker.FileTypeFilter.Add(".mp3");
            fileOpenPicker.FileTypeFilter.Add(".mp4");

            IReadOnlyList<StorageFile> mediaFiles = await fileOpenPicker.PickMultipleFilesAsync();

            if (mediaFiles != null)
            {
                foreach (StorageFile mediaFile in mediaFiles)
                {
                    Title.Text += mediaFile.Name;
                    this.mediaFileInfos.Add(new MediaFileInfo(mediaFile));
                    FileBlock.Items.Add(mediaFile.Name);
                }
            }
            else
            {
                Title.Text = "error open!";
            }
            
            //foreach (MediaFileInfo mediaFile in this.mediaFileInfos)
            //{
            //    Title.Text += mediaFile.name;
            //}
        }

        //播放当前音频
        public async void PlayMedia()
        {
            if (mediaFilePlaying == null)
            {
                mediaFilePlaying = mediaFileInfos[0];
                mediaIndex = 0;

                var stream = await mediaFilePlaying.mediaFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                MediaPlayer.SetSource(stream, mediaFilePlaying.mediaFile.ContentType);
            }
            Title.Text = mediaFilePlaying.name;
        }
        //播放下一音频
        public async void NextMediaAsync()
        {
            ++mediaIndex;
            if (mediaIndex>=mediaFileInfos.Count)
            {
   ;
                mediaIndex = 0;
            }
            mediaFilePlaying = mediaFileInfos[mediaIndex];
            var stream = await mediaFilePlaying.mediaFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            MediaPlayer.SetSource(stream, mediaFilePlaying.mediaFile.ContentType);

            Title.Text = mediaFilePlaying.name;
        }
        //播放上一音频
        public async void LastMediaAsync()
        {
            --mediaIndex;
            if (mediaIndex < 0)
            {
                
                mediaIndex = mediaFileInfos.Count - 1;
            }
            
            mediaFilePlaying = mediaFileInfos[mediaIndex];

            var stream = await mediaFilePlaying.mediaFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            MediaPlayer.SetSource(stream, mediaFilePlaying.mediaFile.ContentType);

            Title.Text = mediaFilePlaying.name;
        }
        //播放特定位置的音频
        public async void PlayMedia(int index)
        {

            mediaFilePlaying =mediaFileInfos[index];
            mediaIndex = index;
            var stream = await mediaFilePlaying.mediaFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            MediaPlayer.SetSource(stream, mediaFilePlaying.mediaFile.ContentType);
        }
        
        //控件的事件
        //播放,下一首,上一首
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PlayMedia();   
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            NextMediaAsync();
        }

        private void LastButton_Click(object sender, RoutedEventArgs e)
        {
            LastMediaAsync();
        }


        private void FileBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PlayMedia(FileBlock.SelectedIndex);
        }

        private void FileBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            PlayMedia(FileBlock.SelectedIndex);
        }

        //
    }
    public class MediaFileInfo
    {   
        public string name { get; }
        public enum mediaType{ mp3,mp4};
        public mediaType type { get; }
        public StorageFile mediaFile;

        public MediaFileInfo(StorageFile file)
        {
            name = file.Name;
            type = (file.FileType == ".mp3" ? mediaType.mp3 : mediaType.mp4);
            
            mediaFile = file;
        }
    }
}
