using System.Collections.Generic;
using UnityEngine;

public class NameGenerator : MonoBehaviour
{

    private List<string> firstNameSyllables;
    private List<string> lastNameSyllables;
    private List<string> appendix;

    private bool initialized = false;
    // Use this for initialization
    private void init()
    {
        firstNameSyllables = new List<string>();
        firstNameSyllables.Add("mon");
        firstNameSyllables.Add("fay");
        firstNameSyllables.Add("shi");
        firstNameSyllables.Add("zag");
        firstNameSyllables.Add("blarg");
        firstNameSyllables.Add("rash");
        firstNameSyllables.Add("izen");

        lastNameSyllables = new List<string>();
        lastNameSyllables.Add("malo");
        lastNameSyllables.Add("zak");
        lastNameSyllables.Add("abo");
        lastNameSyllables.Add("wonk");

        appendix = new List<string>();
        appendix.Add("son");
        appendix.Add(" the third");
        appendix.Add("li");
        appendix.Add("ssen");
        appendix.Add("kor");

        initialized = true;
    }

    public string GenerateName()
    {
        if (!initialized)
        {
            init();
        }
        return CreateNewName() + " "+ CreateNewSurname();
    }

    private string CreateNewName()
    {
        //Creates a first name with 2-3 syllables
        string firstName = "";
        int numberOfSyllablesInFirstName = Random.Range(2, 4);
        for (int i = 0; i < numberOfSyllablesInFirstName; i++)
        {
            firstName += firstNameSyllables[Random.Range(0, firstNameSyllables.Count)];
        }
        string firstNameLetter = "";
        firstNameLetter = firstName.Substring(0, 1);
        firstName = firstName.Remove(0, 1);
        firstNameLetter = firstNameLetter.ToUpper();
        firstName = firstNameLetter + firstName;
        return firstName;
    }

    private string CreateNewSurname()
    {
        string surname = "";
        int numberOfSyllablesInFirstName = Random.Range(1, 3);
        for (int i = 0; i < numberOfSyllablesInFirstName; i++)
        {
            surname += lastNameSyllables[Random.Range(0, lastNameSyllables.Count)];
        }
        string firstNameLetter = "";
        firstNameLetter = surname.Substring(0, 1);
        surname = surname.Remove(0, 1);
        firstNameLetter = firstNameLetter.ToUpper();
        surname = firstNameLetter + surname;
        if (Random.Range(0f, 1f) > 0.7f)
        {
            surname += appendix[Random.Range(0, appendix.Count)];
        }
        return surname;
    }

}
