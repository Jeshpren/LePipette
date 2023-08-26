// See https://aka.ms/new-console-template for more information
using LePipette;
using System.Runtime.CompilerServices;

Pipette _mainPipette = new Pipette(1, "Pepca");


string _pipetteNickName = TestSecondsScript.FindNickname(_mainPipette.name);


Print(_pipetteNickName);

void Print(string printString)
{
    Console.WriteLine(printString);
}
class Pipette
{
    public int id;
    public string name;

    public Pipette(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
}