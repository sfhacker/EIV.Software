/// <summary>
/// 
/// </summary>
namespace EIV.UI.Telerik
{
    using OData.Core.ITrackable;
    using System.Collections.Generic;
    using System;
    using global::Telerik.Windows.Controls;

    /// <summary>
    /// Can we use a ViewModel which involves many different entities?
    /// </summary>
    public sealed class CustomViewModel : ViewModelBase
    {
        private TrackableEntities entityList = null;

        // New version
        public CustomViewModel()
        {
            this.entityList = new TrackableEntities();
        }

        public int TotalChanges
        {
            get
            {
                return this.entityList.Count;
            }
        }

        public TrackableEntities Items
        {
            get
            {
                return this.entityList;
            }
        }
        // Code related to RadGridView
        // to track (or detect) changes made per row
        // the user is on a cell and then presses ENTER without changing anything
        // entity param can be null (e.g. an empty cell in the grid)
        public bool HasModifications(object entity, IDictionary<string, object> oldValues)
        {
            int entityHasModifications = 0;

            if (oldValues == null)
            {
                throw new ArgumentNullException();
            }

            // This piece of code should be placed after the 'Insert' case.
            foreach (var oldValue in oldValues)
            {
                // This code sucks!
                // Several cases here
                if ((entity == null) && (oldValue.Value == null))
                {
                    continue;
                }
                if (entity != null)
                {
                    if (!entity.Equals(oldValue.Value))
                    {
                        entityHasModifications++;
                        continue;
                    }
                }
                if (!oldValue.Value.Equals(entity))
                {
                    entityHasModifications++;
                }
            }

            // simplify here
            if (entityHasModifications == 0)
            {
                return false;
            }

            return true;
        }

        // entity param can refer to 'Pais', 'Persona', 'Asiento', etc
        // We need all three operations (Insert / Update / Delete) due to OData
        // GridView_RowEditEnded (e.EditOperationType == GridViewEditOperationType.Insert)
        public void InsertEntity<T>(object entity) where T : class
        {
            if (entity == null)
            {
                return;
            }

            // 'T' and 'entity' param should be of the same type
            this.entityList.ProcessEntity<T>(entity, TrackableEntity.Operation.Insert);
        }

        // entity param can refer to 'Pais', 'Persona', 'Asiento', etc
        // GridView_RowEditEnded (e.EditOperationType == GridViewEditOperationType.Edit)
        public void UpdateEntity<T>(object entity) where T : class
        {
            if (entity == null)
            {
                return;
            }

            this.entityList.ProcessEntity<T>(entity, TrackableEntity.Operation.Update);
        }

        // Refactor
        // GridViewEvent: GridView_Deleting
        public void RemoveEntities<T>(IEnumerable<object> entities) where T : class
        {
            if (entities == null)
            {
                return;
            }

            foreach (object item in entities)
            {
                this.entityList.ProcessEntity<T>(item, TrackableEntity.Operation.Delete);
            }
        }

        public void ExecuteAction(string entityPath, string actionName, IDictionary<string, object> actionParams)
        {
        }
    }
}