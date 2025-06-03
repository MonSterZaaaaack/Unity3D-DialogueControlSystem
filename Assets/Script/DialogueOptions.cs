using System;
namespace DialogueControl
{
    [Serializable]
    public class DialogueOptions
    {
        public string tableName;
        public string tableKey;
        public int skipTo;
        public DialogueOptions()
        {
            tableName = string.Empty;
            tableKey = string.Empty;
            skipTo = 0;
        }
        public DialogueOptions(DialogueOptions other)
        {
            tableName = other.tableName;
            tableKey = other.tableKey;
            skipTo = other.skipTo;
        }
    }

}
