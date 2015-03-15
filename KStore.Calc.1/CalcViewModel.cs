using KStore.Calc._2.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KStore.Calc._2
{
    public class CalcViewModel : BindableBase, INotifyDataErrorInfo
    {
        private ErrorsContainer<string> _errors;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            return _errors.GetErrors(propertyName);
        }

        public bool HasErrors
        {
            get { return _errors.HasErrors; }
        }

        public CalcViewModel()
        {
            _errors = new ErrorsContainer<string>(OnErrorsChanged);
        }

        private void OnErrorsChanged([CallerMemberName]string propertyName = null)
        {
            var handler = this.ErrorsChanged;
            if (handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        protected void ValidateProperty(object value, [CallerMemberName]string propertyName = null)
        {
            var context = new ValidationContext(this) 
            { 
                MemberName = propertyName 
            };
            var validationErrors = new List<ValidationResult>();
            if (!Validator.TryValidateProperty(value, context, validationErrors))
            {
                this._errors.SetErrors(propertyName, validationErrors.Select(error => error.ErrorMessage));
            }
            else
            {
                this._errors.ClearErrors(propertyName);
            }
        }

        private string _leftValue;

        [Required(ErrorMessage = "必須入力です。")]
        [Range(0, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public string LeftValue
        {
            get { return _leftValue; }
            set 
            { 
                this.SetProperty(ref this._leftValue, value);
                this.ValidateProperty(value);
            }
        }
        
        private string _rightValue;

        [Required(ErrorMessage = "必須入力です。")]
        public string RightValue
        {
            get { return _rightValue; }
            set
            { 
                this.SetProperty(ref this._rightValue, value);
                this.ValidateProperty(value);            
            }
        }

        private string _answerValue;
        public string AnswerValue
        {
            get { return _answerValue; }
            set { this.SetProperty(ref this._answerValue, value); }
        }

        private ICommand _calcCommand;

        public ICommand CalcCommand
        {
            get { return this._calcCommand ?? (this._calcCommand = new CalcCommand(this)); }
        }

        internal bool ValidateAllObjects()
        {
            if (!this.HasErrors)
            {
                var context = new ValidationContext(this);
                var validationErrors = new List<ValidationResult>();
                if (Validator.TryValidateObject(this, context, validationErrors))
                {
                    return true;
                }

                var errors = validationErrors.Where(_ => _.MemberNames.Any()).GroupBy(_ => _.MemberNames.First());
                foreach (var error in errors)
                {
                    _errors.SetErrors(error.Key, error.Select(_ => _.ErrorMessage));
                }
            }
            return false;
        }
        
    }

    public class CalcCommand : ICommand
    {
        CalcViewModel _parentViewModel;

        public CalcCommand(CalcViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
        }


        public bool CanExecute(object parameter)
        {
            return _parentViewModel.ValidateAllObjects();
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _parentViewModel.AnswerValue = Util.IntToString(Calculation.Sum(Util.StringToInt(_parentViewModel.LeftValue), Util.StringToInt(_parentViewModel.RightValue)));
        }
    }

}
