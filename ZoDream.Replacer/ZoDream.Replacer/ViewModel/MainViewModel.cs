using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ZoDream.Replacer.Model;
using ZoDream.Helper.Local;

namespace ZoDream.Replacer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
        }

        /// <summary>
        /// The <see cref="Filter" /> property's name.
        /// </summary>
        public const string FilterPropertyName = "Filter";

        private string _filter = @"^\.(php|js|css|txt|html|htm|tpl)$";

        /// <summary>
        /// Sets and gets the Filter property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                Set(FilterPropertyName, ref _filter, value);
            }
        }

        /// <summary>
        /// The <see cref="Message" /> property's name.
        /// </summary>
        public const string MessagePropertyName = "Message";

        private string _message = string.Empty;

        /// <summary>
        /// Sets and gets the Message property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                Set(MessagePropertyName, ref _message, value);
            }
        }

        /// <summary>
        /// The <see cref="IsBak" /> property's name.
        /// </summary>
        public const string IsBakPropertyName = "IsBak";

        private bool _isBak = false;

        /// <summary>
        /// Sets and gets the IsBak property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsBak
        {
            get
            {
                return _isBak;
            }
            set
            {
                Set(IsBakPropertyName, ref _isBak, value);
            }
        }
        

        /// <summary>
        /// The <see cref="Pattern" /> property's name.
        /// </summary>
        public const string PatternPropertyName = "Pattern";

        private string _pattern = string.Empty;

        /// <summary>
        /// Sets and gets the Pattern property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Pattern
        {
            get
            {
                return _pattern;
            }
            set
            {
                Set(PatternPropertyName, ref _pattern, value);
            }
        }

        /// <summary>
        /// The <see cref="Replace" /> property's name.
        /// </summary>
        public const string ReplacePropertyName = "Replace";

        private string _replace = string.Empty;

        /// <summary>
        /// Sets and gets the Replace property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Replace
        {
            get
            {
                return _replace;
            }
            set
            {
                Set(ReplacePropertyName, ref _replace, value);
            }
        }

        /// <summary>
        /// The <see cref="ReplaceList" /> property's name.
        /// </summary>
        public const string ReplaceListPropertyName = "ReplaceList";

        private ObservableCollection<ReplaceItem> _replaceList = new ObservableCollection<ReplaceItem>();

        /// <summary>
        /// Sets and gets the ReplaceList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ReplaceItem> ReplaceList
        {
            get
            {
                return _replaceList;
            }
            set
            {
                Set(ReplaceListPropertyName, ref _replaceList, value);
            }
        }

        /// <summary>
        /// The <see cref="FileList" /> property's name.
        /// </summary>
        public const string FileListPropertyName = "FileList";

        private ObservableCollection<FileItem> _fileList = new ObservableCollection<FileItem>();

        /// <summary>
        /// Sets and gets the FileList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<FileItem> FileList
        {
            get
            {
                return _fileList;
            }
            set
            {
                Set(FileListPropertyName, ref _fileList, value);
            }
        }

        private RelayCommand _startCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand StartCommand
        {
            get
            {
                return _startCommand
                    ?? (_startCommand = new RelayCommand(ExecuteStartCommand));
            }
        }

        private void ExecuteStartCommand()
        {
            _addReplace();
            _begin();
        }

        private RelayCommand<int> _moveUpCommand;

        /// <summary>
        /// Gets the MoveUpCommand.
        /// </summary>
        public RelayCommand<int> MoveUpCommand
        {
            get
            {
                return _moveUpCommand
                    ?? (_moveUpCommand = new RelayCommand<int>(ExecuteMoveUpCommand));
            }
        }

        private void ExecuteMoveUpCommand(int index)
        {
            if (index <= 0 || index >= ReplaceList.Count) return;
            var item = ReplaceList[index];
            ReplaceList[index] = ReplaceList[index - 1];
            ReplaceList[index - 1] = item;
        }

        private RelayCommand<int> _moveDownCommand;

        /// <summary>
        /// Gets the MoveDownCommand.
        /// </summary>
        public RelayCommand<int> MoveDownCommand
        {
            get
            {
                return _moveDownCommand
                    ?? (_moveDownCommand = new RelayCommand<int>(ExecuteMoveDownCommand));
            }
        }

        private void ExecuteMoveDownCommand(int index)
        {
            if (index < 0 || index >= ReplaceList.Count - 1) return;
            var item = ReplaceList[index];
            ReplaceList[index] = ReplaceList[index + 1];
            ReplaceList[index + 1] = item;
        }

        private RelayCommand<int> _deleteReplaceCommand;

        /// <summary>
        /// Gets the DeleteReplaceCommand.
        /// </summary>
        public RelayCommand<int> DeleteReplaceCommand
        {
            get
            {
                return _deleteReplaceCommand
                    ?? (_deleteReplaceCommand = new RelayCommand<int>(ExecuteDeleteReplaceCommand));
            }
        }

        private void ExecuteDeleteReplaceCommand(int index)
        {
            if (index < 0 || index >= ReplaceList.Count) return;
            ReplaceList.RemoveAt(index);
        }

        private RelayCommand _clearReplaceCommand;

        /// <summary>
        /// Gets the ClearReplaceCommand.
        /// </summary>
        public RelayCommand ClearReplaceCommand
        {
            get
            {
                return _clearReplaceCommand
                    ?? (_clearReplaceCommand = new RelayCommand(ExecuteClearReplaceCommand));
            }
        }

        private void ExecuteClearReplaceCommand()
        {
            ReplaceList.Clear();
        }

        private RelayCommand _addCommand;

        /// <summary>
        /// Gets the AddCommand.
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand
                    ?? (_addCommand = new RelayCommand(ExecuteAddCommand));
            }
        }

        private void ExecuteAddCommand()
        {
            _addReplace();
        }

        private void _addReplace()
        {
            if (string.IsNullOrEmpty(Pattern) && string.IsNullOrEmpty(Replace))
            {
                _showMessage("替换规则添加失败！不能都为空！");
                return;
            }
            ReplaceList.Add(new ReplaceItem(Pattern, Replace));
            Pattern = Replace = string.Empty;
            _showMessage("替换规则添加成功！");
        }

        private void _begin()
        {
            if (FileList.Count <= 0 || ReplaceList.Count <= 0)
            {
                _showMessage("请添加规则或文件！");
                return;
            }
            _showMessage("替换开始！");
            _task();
        }

        private void _task()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var item in FileList)
                {
                    _runOne(item);
                }
                _showMessage("替换完成！");
            });
        }

        private void _runOne(FileItem item)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                item.Status = DealStatus.Wating;
            });
            var status = DealStatus.Success;
            var count = 0;
            try
            {
                var fs = new FileStream(item.FullName, FileMode.Open);
                var encoder = new TxtEncoder();
                var reader = new StreamReader(fs, encoder.GetEncoding(fs));
                var content = reader.ReadToEnd();
                reader.Close();
                foreach (var replace in ReplaceList)
                {
                    var regex = new Regex(replace.Search);
                    var tmpcount = regex.Matches(content).Count;
                    if (tmpcount <= 0)
                    {
                        continue;
                    }
                    count += tmpcount;
                    content = regex.Replace(content, replace.Replace);
                }
                if (count <= 0)
                {
                    goto end;
                }
                if (IsBak)
                {
                    File.Copy(item.FullName, item.FullName + ".bak", true);
                }
                var writer = new StreamWriter(item.FullName, false, Encoding.UTF8);
                writer.Write(content);
                writer.Close();
            }
            catch (Exception ex)
            {
                status = DealStatus.Failure;
                _showMessage("可能原因：正则表达式错误!" + ex.Message);
            }

            end:

            Application.Current.Dispatcher.Invoke(() =>
            {
                item.Status = status;
                item.Count = count;
            });
        }

        private void _parallel()
        {
            var result = Parallel.ForEach(FileList, item =>
            {
                item.Status = DealStatus.Wating;
                try
                {
                    var fs = new FileStream(item.FullName, FileMode.Open);
                    var encoder = new TxtEncoder();
                    var reader = new StreamReader(fs, encoder.GetEncoding(fs));
                    var content = reader.ReadToEnd();
                    reader.Close();
                    foreach (var replace in ReplaceList)
                    {
                        var regex = new Regex(replace.Search);
                        var count = regex.Matches(content).Count;
                        if (count <= 0)
                        {
                            continue;
                        }
                        item.Count += count;
                        content = regex.Replace(content, replace.Replace);
                    }
                    if (item.Count <= 0)
                    {
                        return;
                    }
                    if (IsBak)
                    {
                        File.Copy(item.FullName, item.FullName + ".bak", true);
                    }
                    var writer = new StreamWriter(item.FullName, false, Encoding.UTF8);
                    writer.Write(content);
                    writer.Close();
                    item.Status = DealStatus.Success;
                }
                catch (Exception ex)
                {
                    item.Status = DealStatus.Failure;
                    _showMessage("可能原因：正则表达式错误!" + ex.Message);
                }
            });
            Task.Factory.StartNew(() =>
            {
                while (!result.IsCompleted)
                {
                    Thread.Sleep(500);
                }
                _showMessage("替换完成！");
            });
        }

        private RelayCommand<int> _deleteCommand;

        /// <summary>
        /// Gets the DeleteCommand.
        /// </summary>
        public RelayCommand<int> DeleteCommand
        {
            get
            {
                return _deleteCommand
                    ?? (_deleteCommand = new RelayCommand<int>(ExecuteDeleteCommand));
            }
        }

        private void ExecuteDeleteCommand(int index)
        {
            if (index < 0 || index >= FileList.Count) return;
            FileList.RemoveAt(index);
            _showMessage("已删除一个！");
        }

        private RelayCommand _clearCommand;

        /// <summary>
        /// Gets the ClearCommand.
        /// </summary>
        public RelayCommand ClearCommand
        {
            get
            {
                return _clearCommand
                    ?? (_clearCommand = new RelayCommand(ExecuteClearCommand));
            }
        }

        private void ExecuteClearCommand()
        {
            for (int i = FileList.Count - 1; i >= 0; i--)
            {
                if (FileList[i].Status == DealStatus.Success)
                {
                    FileList.RemoveAt(i);
                }
            }
            _showMessage("删除成功的完成！");
        }

        private RelayCommand _clearAllCommand;

        /// <summary>
        /// Gets the ClearAllCommand.
        /// </summary>
        public RelayCommand ClearAllCommand
        {
            get
            {
                return _clearAllCommand
                    ?? (_clearAllCommand = new RelayCommand(ExecuteClearAllCommand));
            }
        }

        private void ExecuteClearAllCommand()
        {
            FileList.Clear();
            _showMessage("已清空");
        }

        private RelayCommand _openFileCommand;

        /// <summary>
        /// Gets the OpenFileCommand.
        /// </summary>
        public RelayCommand OpenFileCommand
        {
            get
            {
                return _openFileCommand
                    ?? (_openFileCommand = new RelayCommand(ExecuteOpenFileCommand));
            }
        }

        private void ExecuteOpenFileCommand()
        {
            _addFile(Open.ChooseFiles());
            _showMessage($"总共有{FileList.Count}个文件！");
        }

        private RelayCommand _openFolderCommand;

        /// <summary>
        /// Gets the OpenFolderCommand.
        /// </summary>
        public RelayCommand OpenFolderCommand
        {
            get
            {
                return _openFolderCommand
                    ?? (_openFolderCommand = new RelayCommand(ExecuteOpenFolderCommand));
            }
        }

        private void ExecuteOpenFolderCommand()
        {
            _addFile(Open.GetAllFile(Open.ChooseFolder()));
            _showMessage($"总共有{FileList.Count}个文件！");
        }

        private RelayCommand<int> _doubleCommand;

        /// <summary>
        /// Gets the DoubleCommand.
        /// </summary>
        public RelayCommand<int> DoubleCommand
        {
            get
            {
                return _doubleCommand
                    ?? (_doubleCommand = new RelayCommand<int>(ExecuteDoubleCommand));
            }
        }

        private void ExecuteDoubleCommand(int index)
        {
            if (index < 0 || index >= FileList.Count)
            {
                return;
            }
           Open.ExploreFile(FileList[index].FullName);

        }

        private RelayCommand _resetCommand;

        /// <summary>
        /// Gets the ResetCommand.
        /// </summary>
        public RelayCommand ResetCommand
        {
            get
            {
                return _resetCommand
                    ?? (_resetCommand = new RelayCommand(ExecuteResetCommand));
            }
        }

        private void ExecuteResetCommand()
        {
            foreach (var item in FileList)
            {
                item.Status = DealStatus.None;
                item.Count = 0;
            }
        }

        private RelayCommand<DragEventArgs> _fileDrogCommand;

        /// <summary>
        /// Gets the FileDrogCommand.
        /// </summary>
        public RelayCommand<DragEventArgs> FileDrogCommand
        {
            get
            {
                return _fileDrogCommand
                    ?? (_fileDrogCommand = new RelayCommand<DragEventArgs>(ExecuteFileDrogCommand));
            }
        }

        private void ExecuteFileDrogCommand(DragEventArgs parameter)
        {
            if (parameter == null)
            {
                return;
            }
            Array files = (System.Array)parameter.Data.GetData(DataFormats.FileDrop);
            foreach (string item in files)
            {
                if (File.Exists(item))
                {
                    _addOne(item);
                }
                else if (Directory.Exists(item))
                {
                    _addFile(Open.GetAllFile(item));
                }
            }
            _showMessage($"总共有{FileList.Count}个文件！");
        }

        private void _addFile(IList<string> files)
        {
            foreach (var item in files)
            {
                _addOne(item);
            }
        }

        private void _addOne(string file)
        {
            var item = new FileItem(file);
            if (Regex.IsMatch(item.Extension, Filter))
            {
                FileList.Add(item);
            }
        }

        private void _showMessage(string message)
        {
            Message = message;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(3000);
                Message = string.Empty;
            });
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}