
namespace EIV.Demo.Model
{
    using EIV.Demo.Model.Base;
    using System.Collections.Generic;

    public class Country : EntityBase
    {
        private string name;
        private string ddi;

        private IList<State> states = null;

        public Country()
        {
            this.states = new List<State>();
        }
        #region Properties

        public virtual string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        public virtual string DDI
        {
            get
            {
                return this.ddi;
            }

            set
            {
                this.ddi = value;
            }
        }

        public IList<State> States
        {
            get
            {
                return this.states;
            }
            protected set
            {

            }
        }
        #endregion
    }
}