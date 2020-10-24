using L5.Command;
using L5.Converter;
using L5.Model;
using L5.Windows;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace L5.ViewModel
{
    public delegate void EmpDelegate(int idDep, int id, string name, int age, double salary);
    public delegate void DepDelegate(int id, string name);

    /// <summary>
    /// ViewModel главного окна
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// База анных
        /// </summary>
        GBEntities gbe = new GBEntities();

        #region Свойства
        /// <summary>
        /// Коллекция сотрудников
        /// </summary>
        private ObservableCollection<Employee> items { get; set; }

        public ObservableCollection<Employee> Items
        {
            get => items;
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }
        /// <summary>
        /// Выбранный из списка сотрудник
        /// </summary>
        private Employee selectedEmp;
        public Employee SelectedEmp
        {
            get { return selectedEmp; }
            set
            {
                selectedEmp = value;
                OnPropertyChanged("SelectedEmp");
            }
        }
        /// <summary>
        /// Коллекция отделов
        /// </summary>
        public ObservableCollection<Department> ItemsD { get; set; }

        /// <summary>
        /// Выбранный из списка отдел
        /// </summary>
        private Department selectedDep;
        public Department SelectedDep
        {
            get { return selectedDep; }
            set
            {
                selectedDep = value;
                OnPropertyChanged("SelectedDep");
            }
        } 
        #endregion

        #region Constructor
        /// <summary>
        /// Конструктор
        /// </summary>
        public MainViewModel()
        {
            var rnd = new Random();
            // Заполнение списка отделов
            ItemsD = new ObservableCollection<Department>();
            // Первоначальное заполнение таблицы отделов
            if (gbe.Dep.Count() < 1)
            {
                for (int i = 0; i < 4; i++)
                    gbe.Dep.Add(new Dep() { Name = $"Отдел {rnd.Next(1, 10)}"});
                gbe.SaveChanges();
            }
            // Заполнение коллекции отделами. 
            foreach (var item in gbe.Dep)
                ItemsD.Add(new Department {Id = item.Id, Name = item.Name });

            // Заполнение списка сотрудников
            FullEmpoyees();
        }
        #endregion

        #region Методы
        /// <summary>
        /// Заполнение списка сотрудников
        /// </summary>
        private void FullEmpoyees()
        {
            // Определяю минимальное и максимальное значение ключа записей таблицы отделов 
            // (типа сохранить хоть какое-то подобие связности таблиц, необходимое для фильтрации сотрудников по отделу) 
            int minId = (gbe.Dep.OrderBy(d => d.Id).FirstOrDefault()).Id;
            int maxId = (gbe.Dep.OrderByDescending(d => d.Id).FirstOrDefault()).Id;
            Items = new ObservableCollection<Employee>();
            // Первоначальное заполнение таблицы сотрудников
            if (gbe.Emp.Count() < 1)
            {
                var rnd = new Random();
                for (int i = 0; i < 10; i++)
                    gbe.Emp.Add(new Emp() { IdDep = rnd.Next(minId,maxId), Name = $"Сотрудник {rnd.Next(1,100)}", Age = rnd.Next(1,100), Salary = rnd.Next(5000,25000) });
                gbe.SaveChanges();
            }
            // Заполнение коллекции сотрудниками. Оно же обновление грида
            foreach (var item in gbe.Emp)
                Items.Add(new Employee { IdDep = item.IdDep, Id = item.Id, Name = item.Name, Age = item.Age, Salary = item.Salary });
        } 
        #endregion

        #region Команды
        // Команда добавления нового сотрудника
        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      var upW = new UpdateWindow()
                      {
                          Title = "Добавление сотрудника",
                          DataContext = new UpdateViewModel(new Employee(), new EmpDelegate(InEmp))
                      };
                      upW.cbDep.ItemsSource = ItemsD;
                      upW.ShowDialog();

                  }));
            }
        }
        /// <summary>
        /// Добавление нового сотрудника
        /// </summary>
        /// <param name="idDep"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <param name="salary"></param>
        public void InEmp(int idDep, int id, string name, int age, double salary)
        {
            // Добавление записи в таблицу
            gbe.Emp.Add(new Emp() { IdDep = idDep, Name = name, Age = age, Salary = salary });
            gbe.SaveChanges();
            // Добавление объекта в коллекцию. Синхронизация ключа (Id)
            Items.Add(new Employee() { IdDep = idDep, Id = (gbe.Emp.OrderByDescending(e => e.Id).FirstOrDefault()).Id, Name = name, Age = age, Salary = salary });
        }

        // Команда изменения данных сотрудника
        private RelayCommand changeCommand;
        public RelayCommand ChangedCommand
        {
            get
            {
                return changeCommand ??
                  (changeCommand = new RelayCommand(obj =>
                  {
                      if (SelectedEmp != null)
                      {
                          var upW = new UpdateWindow()
                          {
                              Title = "Изменение данных сотрудника",
                              DataContext = new UpdateViewModel(SelectedEmp, new EmpDelegate(ChEmp))
                          };
                          upW.cbDep.ItemsSource = ItemsD;
                          upW.ShowDialog();
                      }
                      else
                          MessageBox.Show("Выберите сотрудника из списка для редактирования");
                  }));
            }
        }
        /// <summary>
        /// Изменение данных сотрудника
        /// </summary>
        /// <param name="idDep"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <param name="salary"></param>
        private void ChEmp(int idDep, int id, string name, int age, double salary)
        {
            if (SelectedEmp != null)
            {
                // Получение, изменение и запись в таблицу соответствующей записи
                var chEmp = gbe.Emp.Where(e => e.Id == id).FirstOrDefault();
                chEmp.IdDep = idDep;
                chEmp.Name = name;
                chEmp.Age = age;
                chEmp.Salary = salary;
                gbe.SaveChanges();
                // Изменение объекта в коллекции
                SelectedEmp.IdDep = idDep;
                SelectedEmp.Id = id;
                SelectedEmp.Name = name;
                SelectedEmp.Age = age;
                SelectedEmp.Salary = salary;
            }
        }

        // Команда удаления сотрудника
        private RelayCommand removeCommand;
        public RelayCommand RemoveCommand => removeCommand ??
                    (removeCommand = new RelayCommand(obj =>
                    {
                        if (obj is Employee emp)
                        {
                            // Удаление объекта из коллекции
                            Items.Remove(emp);

                            //var delEmp = new Emp { Id = emp.Id };
                            //gbe.Emp.Attach(delEmp);
                            //gbe.Emp.Remove(delEmp);

                            // Удаление записи из таблицы
                            gbe.Emp.Remove(gbe.Emp.Where(e => e.Id == emp.Id).FirstOrDefault());
                            gbe.SaveChanges();
                        }
                        else
                            MessageBox.Show("Выберите сотрудника из списка для удаления");
                    },
                    (obj) => Items.Count > 0));

        // Команда добавления нового отдела
        private RelayCommand addDepCommand;
        public RelayCommand AddDepCommand
        {
            get
            {
                return addDepCommand ??
                  (addDepCommand = new RelayCommand(obj =>
                  {
                      var upWDep = new UpdateWindowDep()
                      {
                          Title = "Добавление отдела",
                          DataContext = new UpdateViewModelDep(new Department(), new DepDelegate(InDep))
                      };
                      upWDep.ShowDialog();

                  }));
            }
        }
        /// <summary>
        /// Добавление нового отдела
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public void InDep(int id, string name)
        {
            // Добавление записи в таблицу
            gbe.Dep.Add(new Dep() { Name = name });
            gbe.SaveChanges();
            // Добавление объекта в коллекцию. Синхронизация ключа (Id)
            ItemsD.Add(new Department() { Id = (gbe.Dep.OrderByDescending(d => d.Id).FirstOrDefault()).Id, Name = name });
        }

        // Команда изменения данных отдела
        private RelayCommand changeDepCommand;
        public RelayCommand ChangedDepCommand
        {
            get
            {
                return changeDepCommand ??
                  (changeDepCommand = new RelayCommand(obj =>
                  {
                      if (SelectedDep != null)
                      {
                          var upWDep = new UpdateWindowDep()
                          {
                              Title = "Изменение данных отдела",
                              DataContext = new UpdateViewModelDep(SelectedDep, new DepDelegate(ChDep))
                          };
                          upWDep.ShowDialog();
                      }
                      else
                          MessageBox.Show("Выберите отдел из списка для редактирования");
                  }));
            }
        }
        /// <summary>
        /// Изменение данных отдела
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        private void ChDep(int id, string name)
        {
            if (SelectedDep != null)
            {

                // Получение, изменение и запись в таблицу соответствующей записи
                var chDep = gbe.Dep.Where(d => d.Id == id).FirstOrDefault();
                chDep.Name = name;
                gbe.SaveChanges();
                // Изменение объекта в коллекции
                SelectedDep.Name = name;
            }
        }

        // Команда удаления отдела
        private RelayCommand removeDepCommand;
        public RelayCommand RemoveDepCommand => removeDepCommand ??
                    (removeDepCommand = new RelayCommand(obj =>
                    {
                        if (obj is Department dep)
                        {
                            ItemsD.Remove(dep);
                            gbe.Dep.Remove(gbe.Dep.Where(d => d.Id == dep.Id).FirstOrDefault());
                            gbe.SaveChanges();
                        }
                        else
                            MessageBox.Show("Выберите отдел из списка для удаления");
                    },
                    (obj) => ItemsD.Count > 0));


        // Команда фильтрации сотрудников
        private RelayCommand filterDepCommand;
        public RelayCommand FilterDepCommand => filterDepCommand ??
                    (filterDepCommand = new RelayCommand(obj =>
                    {
                        if (obj is Department dep)
                        {
                            var filtr = Items.Where(i => i.IdDep == (SelectedDep).Id).ToObservableCollection();
                            Items = filtr;
                        }
                        else
                            MessageBox.Show("Выберите отдел из списка для отбора сотрудников");
                    }));

        // Команда восстановления списка сотрудников после фильтрации
        private RelayCommand refreshEmpCommand;
        public RelayCommand RefreshEmpCommand => refreshEmpCommand ??
                    (refreshEmpCommand = new RelayCommand(obj =>
                    {
                        FullEmpoyees();
                    }));
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}