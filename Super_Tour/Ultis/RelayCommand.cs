using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace Student_wpf_application.ViewModels.Command
{
    internal class RelayCommand : ICommand
    {
        readonly Action<Object> execute;
        readonly Predicate<Object> can_execute;

        public RelayCommand(Action<object> execute, Predicate<object> can_execute)
        {
            if (execute == null)
            {
                throw new NullReferenceException("execute");
            }
            this.execute = execute;
            this.can_execute = can_execute;
        }
        public RelayCommand(Action<object> execute):this(execute,null)
        {

        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested+= value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return can_execute == null ? true : can_execute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
