using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CustomStudySet : IEnumerable
{
    public string ID { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Creator { get; private set; }
    public int WordCount { get; private set; }
    public int Uses { get; private set; }
    public string Info { get; private set; }
    public DateTime CreationDate { get; private set; }
    public DateTime LastModifiedDate { get; private set; }



    /*Properties for the PlayFab intermediate:
     * CreatorEntity
     * DisplayProperties a serialization of the CustomStudySet object
     * DisplayVerison
     * Type
    */

    private List<JapaneseWord> contents = new List<JapaneseWord>();
    private string[] tags = new string[32];

    public CustomStudySet() 
    {

        Info = Creator + " " + Name + " " + Description + " " + String.Join(",",tags);
    } 


   

    public IEnumerator GetEnumerator()
    {
        return contents.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
