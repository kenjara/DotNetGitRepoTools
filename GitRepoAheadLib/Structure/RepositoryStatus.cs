namespace GitRepoAheadLib.Structure
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class RepositoryStatus : INotifyPropertyChanged
    {
        private string name = string.Empty;

        private bool ahead = false;

        private bool behind = false;

        private string path = string.Empty;

        public RepositoryStatus(string nameIn, string pathIn)
        {
            this.name = nameIn;
            this.path = pathIn;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.OnPropertyChanged();
            }
        }

        public bool Ahead
        {
            get
            {
                return this.ahead;
            }
            set
            {
                this.ahead = value;
                this.OnPropertyChanged();
            }
        }
        // Not yet implemented.
        //public bool Behind
        //{
        //    get
        //    {
        //        return this.behind;
        //    }
        //    set
        //    {
        //        this.behind = value;
        //        this.OnPropertyChanged();
        //    }
        //}

        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
