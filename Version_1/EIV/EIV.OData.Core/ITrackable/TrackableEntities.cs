/// <summary>
/// 
/// </summary>
namespace EIV.OData.Core.ITrackable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class TrackableEntities
    {
        private IList<TrackableEntity> items = null;

        // Multiple different entities (is this possible?)
        public TrackableEntities()
        {
            this.items = new List<TrackableEntity>();
        }

        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        public IList<TrackableEntity> Items
        {
            get
            {
                return this.items;
            }
        }
        private void Add(TrackableEntity entity)
        {
            if (entity == null)
            {
                return;
            }

            this.items.Add(entity);
        }

        private void Remove(object entity)
        {
            if (entity == null)
            {
                return;
            }

            var rst = this.items.Where(x => x.CompareTo(entity)).ToList();
            if (rst == null)
            {
                return;
            }
            if (rst.Count != 1)
            {
                return;
            }

            this.items.Remove(rst[0]);
        }

        public void Clear()
        {
            this.items.Clear();
        }

        // Before: TrackableEntity entity
        // entity: Pais, Persona, Localidad, etc.
        private bool Contains(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentException();
            }

            if (this.items.Count == 0)
            {
                return false;
            }
            foreach (TrackableEntity item in this.items)
            {
                if (item.CompareTo(entity))
                {
                    return true;
                }
            }

            return false;
        }

        // All possible combinations here!
        // Insert --> Update
        //        --> Delete
        //
        // Update --> Delete
        // Update --> Update
        //
        // Some magic here!!
        public void ProcessEntity<T>(object entity, TrackableEntity.Operation operation) where T : class
        {
            if (entity == null)
            {
                return;
            }
            if (operation == TrackableEntity.Operation.Unknown)
            {
                return;
            }

            Type classType = typeof(T);
            Type entityType = entity.GetType();

            // Paranoic!
            if (!classType.Equals(entityType))
            {
                return;
            }
            bool contains = this.Contains(entity);
            if (!contains)
            {
                this.items.Add(new TrackableEntity(entity, operation));

                return;
            }

            var rst = this.items.Where(x => x.CompareTo(entity)).ToList();
            if (rst == null)
            {
                return;
            }
            if (rst.Count != 1)
            {
                return;
            }

            TrackableEntity thisEntity = rst[0] as TrackableEntity;
            if (operation == TrackableEntity.Operation.Delete)
            {
                if (thisEntity.OperationType == TrackableEntity.Operation.Insert)
                {
                    this.items.Remove(thisEntity);

                    return;
                }
                if (thisEntity.OperationType == TrackableEntity.Operation.Update)
                {
                    this.items.Remove(thisEntity);

                    this.items.Add(new TrackableEntity(entity, operation));

                    return;
                }
            }
            if (operation == TrackableEntity.Operation.Update)
            {
                if (thisEntity.OperationType == TrackableEntity.Operation.Insert)
                {
                    this.items.Remove(thisEntity);

                    this.items.Add(new TrackableEntity(entity, TrackableEntity.Operation.Insert));

                    return;
                }
                if (thisEntity.OperationType == TrackableEntity.Operation.Update)
                {
                    this.items.Remove(thisEntity);

                    this.items.Add(new TrackableEntity(entity, operation));

                    return;
                }
            }
        }
    }
}