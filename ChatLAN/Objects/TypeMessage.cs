using System;

namespace ChatLAN.Objects;

[Serializable]
public class TypeMessage<TObject>
{
    public TObject Obj;
    public Util.TypeSoketMessage TypeSoketMessage;

    public TypeMessage(TObject obj, Util.TypeSoketMessage typeSoketMessage)
    {
        Obj = obj;
        TypeSoketMessage = typeSoketMessage;
    }

    public TypeMessage()
    {
    }
}
