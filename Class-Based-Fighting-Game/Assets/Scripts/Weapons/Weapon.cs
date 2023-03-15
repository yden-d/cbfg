using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Weapon
{

    void lightDirectional(bool facingRight);
    void lightNonDirectional(bool facingRight);
    void lightUp();
    void lightDown();
    void heavyDirectional(bool facingRight);
    void heavyNonDirectional(bool facingRight);
    void heavyUp();
    void heavyDown();

    enum Weapon { 
        Bow,
        Sword,
        Shield,
        Wand,
        Staff
    };
}
