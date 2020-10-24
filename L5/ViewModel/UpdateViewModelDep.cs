using L5.Command;
using L5.Model;
using L5.Windows;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace L5.ViewModel
{
    /// <summary>
    /// ViewModel окна обработки данных отдела
    /// </summary>
    public class UpdateViewModelDep : INotifyPropertyChanged
    {
        #region Делегаты
        public EventHandler CloseHandler;
        private DepDelegate inDep;
        #endregion

        #region Свойства
        /// <summary>
        /// Отдел
        /// </summary>
        private Department dep;
        /// <summary>
        /// Ид отдела
        /// </summary>
        public int Id
        {
            get => dep.Id;
            set { dep.Id = value; OnPropertyChanged("Id"); }
        }
        /// <summary>
        /// Название отдела
        /// </summary>
        public string Name
        {
            get => dep.Name;
            set { dep.Name = value; OnPropertyChanged("Name"); }
        }
        /// <summary>
        /// 
        /// </summary>
        private bool? dialogResult;
        public bool? DialogResult
        {
            get { return dialogResult; }
            protected set
            {
                dialogResult = value;
                OnPropertyChanged("DialogResult");
            }
        }
        #endregion

        #region Конструктор
        /// <summary>
        /// Конструктор
        /// </summary>
        public UpdateViewModelDep(Department dep, DepDelegate d)
        {
            this.dep = dep;
            inDep = d;
        }
        #endregion

        #region Команды
        // Команда отмены изменений и закрытия окна
        private RelayCommand okCommand;
        public RelayCommand OkCommand
        {
            get
            {
                return okCommand ??
                  (okCommand = new RelayCommand(obj =>
                  {
                      if (Id.ToString() != "" && Name != "")
                      {
                          inDep(Id, Name);
                      }



                      DialogResult = true;
                  }));
            }
        }

        // Команда отмены изменений и закрытия окна
        private RelayCommand cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                return cancelCommand ??
                  (cancelCommand = new RelayCommand(obj =>
                  {
                      DialogResult = true;
                  }));
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}