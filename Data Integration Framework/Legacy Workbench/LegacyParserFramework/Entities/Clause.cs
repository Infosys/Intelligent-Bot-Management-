using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.LegacyWorkbench.Entities
{
    public class Clause
    {
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        int minCount;

        public int MinCount
        {
            get { return minCount; }
            set { minCount = value; }
        }


        int maxCount;

        public int MaxCount
        {
            get { return maxCount; }
            set { maxCount = value; }
        }

        GenericCollection<Clause> clauses = new GenericCollection<Clause>();

        public GenericCollection<Clause> Clauses
        {
            get { return clauses; }
            set { clauses = value; }
        }

        Entities.Entity clauseEntity;

        public Entities.Entity ClauseEntity
        {
            get { return clauseEntity; }
            set { clauseEntity = value; }
        }


        string hostName;

        public string HostName
        {
            get
            {
                if (hostName == null || hostName.Length == 0)
                {
                    return name;
                }
                else
                {
                    return hostName;
                }
            }
            set { hostName = value; }
        }

    }
}
