using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace L5.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Employee : INotifyPropertyChanged
    {
        #region Свойства
        private int idDep;
        /// <summary>
        /// Id отдела
        /// </summary>
        public int IdDep
        {
            get { return idDep; }
            set
            {
                if (idDep != value)
                {
                    idDep = value;
                    OnPropertyChanged("IdDep");
                }
            }
        }

        private int id;
        /// <summary>
        /// Id сотрудника
        /// </summary>
        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        private string name;
        /// <summary>
        /// ФИО сотрудника
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        private int age;
        /// <summary>
        /// Возраст сотрудника
        /// </summary>
        public int Age
        {
            get { return age; }
            set
            {
                if (age != value)
                {
                    age = value;
                    OnPropertyChanged("Age");
                }
            }
        }

        private double salary;
        /// <summary>
        /// Зарплата сотрудника
        /// </summary>
        public double Salary
        {
            get { return salary; }
            set
            {
                if (salary != value)
                {
                    salary = value;
                    OnPropertyChanged("Salary");
                }
            }
        } 
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public Employee() { }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}