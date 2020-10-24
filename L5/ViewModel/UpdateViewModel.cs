using L5.Command;
using L5.Model;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace L5.ViewModel
{
    /// <summary>
    /// ViewModel окна обработки данных сотрудника
    /// </summary>
    public class UpdateViewModel : INotifyPropertyChanged
    {
        #region Делегаты
        public EventHandler CloseHandler;
        private EmpDelegate inEmp;
        #endregion

        #region Свойства
        /// <summary>
        /// Сотрудник
        /// </summary>
        private Employee employee;
        /// <summary>
        /// Ид отдела
        /// </summary>
        public int IdDep
        {
            get => employee.IdDep;
            set { employee.IdDep = value; OnPropertyChanged("IdDep"); }
        }
        /// <summary>
        /// Ид сотрудника
        /// </summary>
        public int Id
        {
            get => employee.Id;
            set { employee.Id = value; OnPropertyChanged("Id"); }
        }
        /// <summary>
        /// ФИО сотрудника
        /// </summary>
        public string Name
        {
            get => employee.Name;
            set { employee.Name = value; OnPropertyChanged("Name"); }
        }
        /// <summary>
        /// Возраст сотрудника
        /// </summary>
        public int Age
        {
            get => employee.Age;
            set { employee.Age = value; OnPropertyChanged("Age"); }
        }
        /// <summary>
        /// Зарплата сотрудника
        /// </summary>
        public double Salary
        {
            get => employee.Salary;
            set { employee.Salary = value; OnPropertyChanged("Salary"); }
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
        public UpdateViewModel(Employee e, EmpDelegate d)
        {
            employee = e;
            inEmp = d;
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
                      if (IdDep.ToString() != "" && Id.ToString() != "" && Name != "" && Age.ToString() != null && Salary.ToString() != null)
                      {
                          inEmp(IdDep, Id, Name, Age, Salary);
                      }



                      DialogResult = false;
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
                      //CloseWindow();
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