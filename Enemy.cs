using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe abstraite pour gérer tous les ennemis
[System.Serializable]
public abstract class Enemy : MonoBehaviour
{
    // Variable servant à savoir si l'ennemi est stun
    public bool isStun;
    // Dégâts de l'ennemi au joueur
    public int damageAmount;
    // Méthode pour stun l'ennemi
    public abstract void Stun();
    // Méthode pour destun l'ennemi
    public abstract void Destun();
    // Méthode pour faire déplacer l'ennemi
    public abstract void Move();
    // Méthode pour utiliser le geyser sur l'ennemi
    public abstract void Geyser();
}

