using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MadagascarVanilla
{

    // From ExportAgency (https://steamcommunity.com/sharedfiles/filedetails/?id=1467209473)
    public class ExposableList<T> : List<ExposableListItem<T>>, IExposable where T : IExposable
    {
        public ExposableList() : base(capacity: 1)
        {
            Log.Message("ExposableListConstructor");
        }

        public ExposableList(IEnumerable<T> exposables) : base(collection: exposables.Select(selector: exp => new ExposableListItem<T>(exp)))
        {
            Log.Message("ExposableListConstructor with collection");
        }

        public string Name { get; set; }

        public void ExposeData()
        {
            string name = this.Name;
            Scribe_Values.Look(value: ref name, label: "name");
            this.Name = name;

            List<ExposableListItem<T>> list = this.ListFullCopy();
            Scribe_Collections.Look(list: ref list, label: "internalList");
            this.Clear();
            this.AddRange(collection: list.Where(predicate: exp => exp.resolvable));
        }

        internal void Add(T item) => base.Add(item: new ExposableListItem<T>(exposable: item));
    }

    public class ExposableListItem<T> : IExposable where T : IExposable
    {
        public T exposable;
        public bool resolvable;

        public ExposableListItem()
        {
        }

        public ExposableListItem(T exposable) => this.exposable = exposable;

        public void ExposeData()
        {
            try
            {
                Scribe_Deep.Look(target: ref this.exposable, label: "exposable");
                this.resolvable = this.exposable != null;
            }
            catch
            {
                this.resolvable = false;
            }

            if (!this.resolvable)
                Log.Message(text: $"Found unresolvable {typeof(T).FullName} in exported List");
        }

        public static implicit operator T(ExposableListItem<T> exp) => exp.exposable;
    }
}