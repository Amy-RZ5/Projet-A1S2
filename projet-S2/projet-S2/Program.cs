// See https://aka.ms/new-console-template for more information

using System.Drawing;
using System.Reflection;


internal class Program
{
    static public Random rand = new Random();
    
    public struct Cellule
    {
        public bool etat = false; // false : n'est pas en feu
        public string type;
        public int delai;
        public string symbole;
        public bool etaitFeu = false; // false : n'était pas en feu au tour d'avant

        public Cellule(string type, int delai, string symbole)
        {
            this.type = type;
            this.delai = delai;
            this.symbole = symbole;
        }
    }
    
    static public Cellule[,] RemplissageAleatoire()
    {
        int choix;

        int ligne = SaisieSecu();
        int colonne = SaisieSecu();
        
        Cellule[,] matrice = new Cellule[ligne,colonne];
        for (int i = 0; i < ligne; i++)
        {
            for (int j = 0; j < colonne; j++)
            {
                choix = rand.Next(2, 3);
                switch (choix)
                {
                    case 1:
                        matrice[i, j] = new Cellule("herbe", 8, "🌿");
                        break;
                    case 2:
                        matrice[i, j] = new Cellule("arbre", 10, "🌳");
                        break;
                    case 3:
                        matrice[i, j] = new Cellule("terrain", 0, "🟫");
                        break;
                    case 4:
                        matrice[i, j] = new Cellule("feuille", 4, "🍁");
                        break;
                    case 5:
                        matrice[i, j] = new Cellule("eau", 0, "💧");
                        break;
                    case 6:
                        matrice[i, j] = new Cellule("rocher", 50, "🪨");
                        break;
                    
                    
                }
            }
        }

        return matrice;
    }

    #region Affichage console
    
    private static void Affichage(Cellule[,] plateau)
    {
        for (int i = 0; i < plateau.GetLength(0); i++)
        { 
            for (int j = 0; j < plateau.GetLength(1); j++)
            {
                if (plateau[i, j].etat) Console.BackgroundColor = ConsoleColor.Red;
                Console.Write(plateau[i,j].symbole);
                Console.ResetColor();
                Console.Write(" ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public static int DebutJeu()
    {
        Console.WriteLine("Bonjour ! Bienvenue sur FireConquest, le jeu de simulation iconique !");
        Console.WriteLine("Quel est le mode de jeu que vous sélectionnez ?\n" +
                          "1.\t Appuyer jusqu'à bon vous semble\n" +
                          "2.\t Arrêter quand toute la matrice est en feu\n");
        int n = SaisieSecu();
        return n;
    }

    #endregion

    #region paramètres et checks
    public static bool PeutEtreEnFeu(int ligne, int colonne, Cellule[,] plateau)
    {
        bool check = true;
        
        if (plateau[ligne, colonne].delai == 0)
        { 
            plateau[ligne, colonne].etat = false;
            plateau[ligne, colonne].etaitFeu = false;
            check = false;
        }

        return check;
    }
    
    public static void ParametresCaseEnFeu(int ligne, int colonne, Cellule[,] plateau)
    {
        if (plateau[ligne, colonne].etat)
        {
            if (plateau[ligne, colonne].delai >= 2)
            {
                plateau[ligne, colonne].delai -= 1;
                plateau[ligne, colonne].etaitFeu = true;
            }
            else
            {
                if (plateau[ligne, colonne].type == "cendres")
                {
                    plateau[ligne, colonne].symbole = "◽️";
                    plateau[ligne, colonne].etat = false;
                    plateau[ligne, colonne].etaitFeu = false;
                    plateau[ligne, colonne].delai = 0;
                    plateau[ligne, colonne].type = "cendres éteintes";
                }
                else
                {
                    plateau[ligne, colonne].type = "cendres";
                    plateau[ligne, colonne].delai = 1;
                    plateau[ligne, colonne].symbole = "🔸";
                    plateau[ligne, colonne].etaitFeu = true;
                }

            }
        }

    }
    
    static int SaisieSecu()
    {
        int n;
        while(!Int32.TryParse(Console.ReadLine(), out n)||n<0)
            Console.WriteLine("Saisie invalide");
        return n;
    }

    public static void FaireBrulerTrue(int ligne, int colonne, Cellule[,] plateau)
    {
        if (PeutEtreEnFeu(ligne, colonne, plateau))
        {
            plateau[ligne, colonne].etaitFeu = true;
            plateau[ligne, colonne].etat = true;
            
            //ParametresCaseEnFeu(ligne,colonne,plateau);

        }
            
    }
    
    public static void FaireBrulerFalse(int ligne, int colonne, Cellule[,] plateau)
    {
        if (PeutEtreEnFeu(ligne, colonne, plateau))
        {
            plateau[ligne, colonne].etaitFeu = false;
            plateau[ligne, colonne].etat = true;
            //ParametresCaseEnFeu(ligne,colonne,plateau);
        }
            
    }
    

    #endregion

    #region mettre le feu
    
    public static void InitialiseFeu(Cellule[,] plateau)
    {
        int limiteIndiceLigne = plateau.GetLength(0);
        int limiteIndiceColonne = plateau.GetLength(1);

        int indiceLigne = rand.Next(0, limiteIndiceLigne);
        int indiceColonne = rand.Next(0, limiteIndiceColonne);
        
        bool peutBruler = PeutEtreEnFeu(indiceLigne, indiceColonne, plateau);
        while (peutBruler == false)
        {
            indiceLigne = rand.Next(0, limiteIndiceLigne);
            indiceColonne = rand.Next(0, limiteIndiceColonne);
            peutBruler = PeutEtreEnFeu(indiceLigne, indiceColonne, plateau);
        }

        plateau[indiceLigne, indiceColonne].etat = true;
        ParametresCaseEnFeu(indiceLigne, indiceColonne, plateau);

    }
    
    // pour la position de j[colonne], évite de faire du copié collé
    public static int PositionDansLigne(int j, Cellule[,] plateau)
    {
        int position = 0;
        // 1 pour le coin à gauche, 2 pour le coin à droite, 3 sinon
        int limiteLigne = plateau.GetLength(0) - 1;
        int limiteColonne = plateau.GetLength(1) -1;
        if (j == 0)
            position = 1;
        else if (j == limiteColonne || j == limiteLigne)
            position = 2;
        else
            position = 3;
        return position;

    }
    

    public static int CasPourFeu(int i, int j, Cellule[,] plateau)
    {
        int positionLigne;
        
        int cas = 0; // selon les cas définis préalablement
        
                // ligne du haut
                if (i == 0)
                {
                    positionLigne = PositionDansLigne(j, plateau);
                    switch (positionLigne)
                    {
                        case 1:
                            cas = 1;// haut gauche
                            break;
                        case 2:
                            cas = 2; // haut droite
                            break;
                        case 3:
                            cas = 3; // ligne haut
                            break;
                            
                    }
                }
                // ligne du bas
                else if (i == plateau.GetLength(0)-1)
                {
                    positionLigne = PositionDansLigne(j, plateau);
                    switch (positionLigne)
                    {
                        case 1:
                            cas = 4; // bas gauche
                            break;
                        case 2:
                            cas = 5; // bas droit
                            break;
                        case 3:
                            cas = 6; // ligne du bas 
                            break;
                    }
                }
                // colonnes + cas lambda
                else
                {
                    positionLigne = PositionDansLigne(j, plateau);
                    switch (positionLigne)
                    {
                        case 1:
                            cas = 7; // colonne de gauche
                            break;
                        case 2:
                            cas = 8; // colonne de droite
                            break;
                        case 3:
                            cas = 9; // lambda
                            break;
                    }
                }

                return cas;
    }

    public static void PropagerFeu1(Cellule[,] plateau)
    {
        /*
         * 1. coin haut gauche
         * 2. coin haut droit
         * 3. ligne du haut (pas les coins)
         * 4. coin bas gauche
         * 5. coin bas droit
         * 6. ligne du bas (pas les coins)
         * 7. colonne de gauche (pas les coins)
         * 8. colonne de droite (pas les coins)
         * 9. cas lambda
         */
        int cas;
        string resultat = "";
        string logs = "";
        int jMax = plateau.GetLength(1) - 1;
        int iMax = plateau.GetLength(0) - 1;
        for (int i = 0; i < plateau.GetLength(0); i++)
        {
            for (int j = 0; j < plateau.GetLength(1); j++)
            {
                if (plateau[i, j].etaitFeu)
                {
                    cas = CasPourFeu(i, j, plateau);
                    Console.WriteLine();
                    switch (cas)
                    {
                        // coin haut gauche
                        case 1:
                            FaireBrulerTrue(i, j, plateau);
                            FaireBrulerTrue(i, jMax, plateau); // à gauche, de l'autre côté
                            FaireBrulerFalse(iMax, j, plateau); // en haut, de l'autre côté
                            FaireBrulerFalse(iMax, jMax, plateau); // diagonale haut gauche, de l'autre côté
                            FaireBrulerFalse(i, j + 1, plateau); // à droite
                            FaireBrulerFalse(i + 1, j, plateau); // en bas
                            FaireBrulerFalse(i + 1, j + 1, plateau); // diagonale bas droite
                            FaireBrulerFalse(i + 1, jMax, plateau); //diagonale bas gauche, côté opposé
                            FaireBrulerFalse(iMax, j + 1, plateau); // diagonale haut droit de l'autre côté
                            break;
                        // coin haut droit
                        case 2:
                            FaireBrulerTrue(i, j, plateau);
                            FaireBrulerTrue(0, 0, plateau); // droite de l'autre côté
                            FaireBrulerFalse(iMax, 0, plateau); // diag haut droite de l'autre côté
                            FaireBrulerFalse(iMax, jMax, plateau); // haut de l'autre côté
                            FaireBrulerTrue(i, j - 1, plateau); // à gauche
                            FaireBrulerFalse(i + 1, j - 1, plateau); // diagonale bas gauche
                            FaireBrulerFalse(i + 1, j, plateau); // en bas
                            FaireBrulerFalse(iMax, j - 1, plateau); // diagonale haut gauche, de l'autre côté
                            FaireBrulerFalse(i + 1, 0, plateau); // diagonale bas droite de l'autre côté
                            break;
                        // haut
                        case 3:
                            FaireBrulerTrue(i, j, plateau);
                            FaireBrulerFalse(iMax, j - 1, plateau); // diagonale haut gauche, de l'autre côté du plateau
                            FaireBrulerFalse(iMax, j, plateau); // haut, de l'autre côté
                            FaireBrulerFalse(iMax, j + 1, plateau); // diagonale haut droite de l'autre côté
                            FaireBrulerTrue(i, j - 1, plateau); // à gauche
                            FaireBrulerFalse(i, j + 1, plateau); // à droite
                            FaireBrulerFalse(i + 1, j - 1, plateau); // diagonale bas gauche
                            FaireBrulerFalse(i + 1, j, plateau); // en bas
                            FaireBrulerFalse(i + 1, j + 1, plateau); // diagonale bas droite
                            break;
                        // coin bas gauche
                        case 4:
                            FaireBrulerFalse(i, j, plateau);
                            FaireBrulerTrue(0, 0, plateau); // bas de l'autre côté
                            FaireBrulerTrue(0, jMax, plateau); // diag bas gauche de l'autre côté
                            FaireBrulerFalse(iMax, jMax, plateau); //  gauche de l'autre côté
                            FaireBrulerTrue(i - 1, j, plateau); // en haut
                            FaireBrulerTrue(i - 1, j + 1, plateau); // diagonale haut droite
                            FaireBrulerFalse(i, j + 1, plateau); // à droite
                            FaireBrulerTrue(0, j + 1, plateau); // diagonale bas droite de l'autre côté
                            FaireBrulerFalse(iMax - 1, jMax, plateau); // diagonale haut gauche de l'autre côté
                            break;
                        // coin bas droit
                        case 5:
                            FaireBrulerFalse(i, j, plateau);
                            FaireBrulerTrue(0, 0, plateau); // diag bas droite de l'autre côté
                            FaireBrulerTrue(i, jMax, plateau); // bas de l'autre côté
                            FaireBrulerTrue(iMax, 0, plateau); // droite de l'autre côté
                            FaireBrulerTrue(iMax, j - 1, plateau); // gauche
                            FaireBrulerTrue(iMax - 1, j - 1, plateau); // diagonale haut gauche
                            FaireBrulerTrue(iMax - 1, j, plateau); // haut
                            FaireBrulerTrue(iMax - 1, 0, plateau); // diagonale haut droite de l'autre côté
                            FaireBrulerTrue(0, jMax - 1, plateau); // diagonale bas gauche de l'autre côté
                            break;
                        // ligne du bas
                        case 6:
                            FaireBrulerFalse(i, j, plateau);
                            FaireBrulerTrue(0,j-1,plateau); // diagonale bas gauche de l'autre côté
                            FaireBrulerTrue(0,j,plateau); // bas de l'autre côté
                            FaireBrulerTrue(0,j+1,plateau);// diag bas droite de l'autre côté
                            FaireBrulerTrue(i-1,j-1, plateau); // diag haut gauche
                            FaireBrulerTrue(i - 1,j,plateau); // haut
                            FaireBrulerTrue(i-1,j+1, plateau); // diag haut droite
                            FaireBrulerTrue(i,j-1,plateau); // gauche
                            FaireBrulerFalse(i,j+1,plateau); // droite
                            //FaireBrulerFalse(i+1,j,plateau); // bas
                            break;
                        // colonne de gauche
                        case 7:
                            FaireBrulerTrue(i, j, plateau);
                            FaireBrulerFalse(i - 1, jMax, plateau); // diagonale haut gauche de l'autre côté
                            FaireBrulerFalse(i, jMax, plateau); // gauche, de l'autre côté
                            FaireBrulerFalse(i + 1, jMax, plateau); // diag bas gauche de l'autre côté
                            FaireBrulerTrue(i - 1, j, plateau); // haut
                            FaireBrulerTrue(i - 1, j + 1, plateau); // diagonale haut droite
                            FaireBrulerFalse(i, j + 1, plateau); // droite
                            FaireBrulerFalse(i + 1, j, plateau); // en bas
                            FaireBrulerFalse(i + 1, j + 1, plateau); // diag bas droite
                            break;
                        //colonne de droite
                        case 8:
                            FaireBrulerFalse(i, j, plateau);
                            FaireBrulerTrue(i - 1, 0, plateau); // diag haut droite, de l'autre côté
                            FaireBrulerTrue(i, 0, plateau); // droite, de l'autre côté
                            FaireBrulerFalse(i+1, 0, plateau); // diag bas droite de l'autre côté
                            FaireBrulerTrue(i - 1, 0, plateau); // diag haut gauche
                            FaireBrulerTrue(i - 1, j, plateau); // en haut
                            FaireBrulerTrue(i, j - 1, plateau); // gauche
                            FaireBrulerFalse(i + 1, j - 1, plateau); // diagonale bas gauche
                            FaireBrulerFalse(i+1,j,plateau); // bas
                            break;
                        // cas normal
                        case 9:
                            FaireBrulerTrue(i, j, plateau);
                            FaireBrulerTrue(i-1,j-1,plateau); // diag haut gauche
                            FaireBrulerTrue(i-1,j,plateau); // haut
                            FaireBrulerTrue(i-1,j+1,plateau); // diag haut droit
                            FaireBrulerTrue(i,j-1,plateau); // gauche
                            FaireBrulerFalse(i,j+1,plateau); // droite
                            FaireBrulerFalse(i+1,j-1,plateau); // diag bas gauche
                            FaireBrulerFalse(i+1,j,plateau); // bas
                            FaireBrulerFalse(i+1,j+1,plateau); // diag bas droite
                            break;
                        default:
                            Console.WriteLine(cas);
                            break;

                    }
                }
                
            }
        }
    }

    /*public static void PropagerFeu(Cellule[,] plateau)
    {
        int nbLignes = plateau.GetLength(0);
        int nbColonnes = plateau.GetLength(1);
        bool VoisinsFeu = false;

        for (int i = 0; i < nbLignes; i++)
        { 
            for (int j = 0; j < nbColonnes; j++)
            { 
                if (plateau[i, j].etat)
                {
                    if (plateau[i, j].etaitFeu)
                    {
                       //cellule en haut à gauche
                       try
                       {
                           
                           if (PeutEtreEnFeu(i - 1, j - 1, plateau))
                           {
                               VoisinsFeu = true;
                               ParametresCaseEnFeu(i-1,j-1,plateau);
                               plateau[i - 1, j - 1].etaitFeu = true;
                           }
                               
                       }
                       catch (IndexOutOfRangeException)
                       {
                           if (j == 0 && i == 0)
                           {
                               
                               if (PeutEtreEnFeu(nbLignes - 1, nbColonnes - 1, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(nbLignes-1,nbColonnes-1, plateau);
                                   plateau[nbLignes - 1, nbColonnes - 1].etaitFeu = false;
                               }
                               if (PeutEtreEnFeu(0, nbColonnes - 1, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(0,nbColonnes-1, plateau);
                                   plateau[0, nbColonnes - 1].etaitFeu = false;
                               }
                               if (PeutEtreEnFeu(nbLignes - 1, 0, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(nbLignes-1,0, plateau);
                                   plateau[nbLignes - 1, 0].etaitFeu = false;
                               }
                           }
                           else if (j == 0 && i != 0)
                           {

                               if (PeutEtreEnFeu(i - 1, nbColonnes- 1, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(i-1,nbColonnes-1, plateau);
                                   plateau[i - 1, nbColonnes-1].etaitFeu = true;
                               }
                           }
                           else if (j!=0 && i==0)
                           {
                               if (PeutEtreEnFeu(nbLignes - 1, j-1, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(nbLignes-1,j-1, plateau);
                                   plateau[nbLignes - 1, j-1].etaitFeu = false;
                               }
                           }
                           
                       }
                       //cellule au-dessus
                       try
                       {
                           
                           if (PeutEtreEnFeu(i - 1, j, plateau))
                           {
                               VoisinsFeu = true;
                               ParametresCaseEnFeu(i-1, j, plateau);
                               plateau[i - 1, j].etaitFeu = true;
                           }
                       }
                       catch (IndexOutOfRangeException)
                       {
                           
                           if (PeutEtreEnFeu(nbLignes - 1, j, plateau))
                           {
                               VoisinsFeu = true;
                               ParametresCaseEnFeu(nbLignes-1,j, plateau);
                               plateau[nbLignes - 1, j].etaitFeu = false;
                           }
                       }
                       // cellule en haut à droite
                       try
                       {
                           if (PeutEtreEnFeu(i - 1, j + 1, plateau))
                           {
                               VoisinsFeu = true;
                               ParametresCaseEnFeu(i-1, j+1, plateau);
                               plateau[i - 1, j + 1].etaitFeu = true;
                           }
                       }
                       catch (IndexOutOfRangeException)
                       {
                           if (j == nbColonnes - 1 && i==0)
                           {

                               if (PeutEtreEnFeu(nbLignes - 1, 0, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(nbLignes-1,0, plateau);
                                   plateau[nbLignes - 1, 0].etaitFeu = false;
                               }
                               if (PeutEtreEnFeu(0, 0, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(0,0, plateau);
                                   plateau[0, 0].etaitFeu = true;
                               }
                               if (PeutEtreEnFeu(nbLignes - 1, nbColonnes-1, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(nbLignes-1,nbColonnes-1, plateau);
                                   plateau[nbLignes - 1, nbColonnes - 1].etaitFeu = false;
                               }
                           }
                           else if (i==0&&j!=nbColonnes-1)
                           {

                               if (PeutEtreEnFeu(nbLignes - 1, j + 1, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(nbLignes-1,j+1, plateau);
                                   plateau[nbLignes - 1, j+1].etaitFeu = false;
                               }
                           }
                           else if (i!=0&&j==nbColonnes-1)
                           {
                               if (PeutEtreEnFeu(i - 1, 0, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(i-1,0, plateau);
                                   plateau[i - 1, 0].etaitFeu = true;
                               }
                               
                           }
                           
                           
                       }
                       // cellule à gauche
                       try
                       {

                           if (PeutEtreEnFeu(i, j - 1, plateau))
                           {
                               VoisinsFeu = true;
                               ParametresCaseEnFeu(i, j-1, plateau);
                               plateau[i, j - 1].etaitFeu = true;
                           }
                       }
                       catch (IndexOutOfRangeException)
                       {

                           if (PeutEtreEnFeu(i, nbColonnes-1, plateau))
                           {
                               VoisinsFeu = true;
                               ParametresCaseEnFeu(i,nbColonnes-1, plateau);
                               plateau[i, nbColonnes-1].etaitFeu = false;
                           }
                       }
                       // cellule à droite
                       try
                       {

                           if (PeutEtreEnFeu(i, j + 1, plateau))
                           {
                               VoisinsFeu = true;
                               ParametresCaseEnFeu(i, j+1, plateau);
                               plateau[i, j + 1].etaitFeu = false;
                           }
                       }
                       catch (IndexOutOfRangeException)
                       {

                           if (PeutEtreEnFeu(i, 0, plateau))
                           {
                               VoisinsFeu = true;
                               ParametresCaseEnFeu(i,0, plateau);
                               plateau[i, 0].etaitFeu = true;
                           }
                       }
                       // cellule en bas à gauche
                       try
                       {

                           if (PeutEtreEnFeu(i + 1, j - 1, plateau))
                           {
                               VoisinsFeu = true;
                               ParametresCaseEnFeu(i+1, j-1, plateau);
                               plateau[i + 1, j - 1].etaitFeu = false;
                           }
                       }
                       catch (IndexOutOfRangeException)
                       {
                           if (j == 0 && i==nbLignes-1)
                           {

                               if (PeutEtreEnFeu(0, nbColonnes - 1, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(0,nbColonnes-1, plateau);
                                   plateau[0, nbColonnes - 1].etaitFeu = true;
                               }
                               if (PeutEtreEnFeu(0, 0, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(0,0, plateau);
                                   plateau[0, 0].etaitFeu = true;
                               }
                               if (PeutEtreEnFeu(nbLignes-1, nbColonnes - 1, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(nbLignes-1,nbColonnes-1, plateau);
                                   plateau[nbLignes-1, nbColonnes - 1].etaitFeu = false;
                               }
                           }
                           else if (i == nbLignes - 1 && j != 0)
                           {
                               if (PeutEtreEnFeu(0, j - 1, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(0,j-1, plateau);
                                   plateau[0, j-1].etaitFeu = true;
                               }
                           }
                           else if (i != nbLignes - 1 && j == 0)
                           {
                               if (PeutEtreEnFeu(i+1, nbColonnes - 1, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(i+1,nbColonnes-1, plateau);
                                   plateau[i+1, nbColonnes-1].etaitFeu = false;
                               }
                               
                           }
                       }
                       // cellule en bas
                       try
                       {

                           if (PeutEtreEnFeu(i + 1, j, plateau))
                           {
                               VoisinsFeu = true;
                               ParametresCaseEnFeu(i+1, j, plateau);
                               plateau[i + 1, j].etaitFeu = false;
                           }
                       }
                       catch (IndexOutOfRangeException)
                       {

                           if (PeutEtreEnFeu(0, j, plateau))
                           {
                               VoisinsFeu = true;
                               ParametresCaseEnFeu(0,j, plateau);
                               plateau[0, j].etaitFeu = true;
                           }
                       }
                       // cellule en bas à droite
                       try
                       {

                           if (PeutEtreEnFeu(i + 1, j + 1, plateau))
                           {
                               VoisinsFeu = true;
                               ParametresCaseEnFeu(i+1, j+1, plateau);
                               plateau[i + 1, j + 1].etaitFeu = false;
                           }
                       }
                       catch (IndexOutOfRangeException)
                       {
                           if (j == nbColonnes - 1 && i ==nbLignes-1)
                           {
                               if (PeutEtreEnFeu(0, 0, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(0,0, plateau);
                                   plateau[0, 0].etaitFeu = true;
                               }
                               if (PeutEtreEnFeu(nbLignes-1, 0, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(nbLignes-1,0, plateau);
                                   plateau[nbLignes-1, 0].etaitFeu = true;
                               }
                               if (PeutEtreEnFeu(0, nbColonnes-1, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(0,nbColonnes-1, plateau);
                                   plateau[0, nbColonnes-1].etaitFeu = true;
                               }
                           }
                           else if(j!=nbColonnes-1&&i==nbLignes-1)
                           {

                               if (PeutEtreEnFeu(0, j + 1, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(0,j+1, plateau);
                                   plateau[0, j+1].etaitFeu = true;
                               }
                           }
                           else if (j==nbColonnes-1 && i!=nbLignes-1)
                           {
                               if (PeutEtreEnFeu(i+1,  0, plateau))
                               {
                                   VoisinsFeu = true;
                                   ParametresCaseEnFeu(i+1,0, plateau);
                                   plateau[i+1, 0].etaitFeu = false;
                               }
                           }
                       }
                    }


                    if (!VoisinsFeu)
                    {
                        if (PeutEtreEnFeu(i, j, plateau))
                        {
                            ParametresCaseEnFeu(i, j, plateau);
                            plateau[i, j].etaitFeu = true;
                            
                        }
                    }
                    plateau[i, j].etaitFeu = true;
                }
            }
            
        }
    }*/
    #endregion

    #region Simulation

    public static void Simulation(Cellule[,] plateau)
    {
        
        Console.WriteLine();
        
        PropagerFeu1(plateau);
        /*for (int i = 0; i < plateau.GetLength(0); i++)
        {
            for(int j = 0;j<plateau.GetLength(1);j++)
            {
                ParametresCaseEnFeu(i,j,plateau);
            }
        }*/
        //Affichage(plateau);
        
        Console.WriteLine();
        
        Affichage(plateau);
    }

    #endregion simulation

    #region debug

    static void LogsCase(Cellule[,] plateau)
    {
        Console.WriteLine("Numéro de ligne de la case souhaitée ?");
        int ligne = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Numéro de colonne de la case souhaitée ?");
        int colonne = Convert.ToInt32(Console.ReadLine());
        
        Console.WriteLine("Les logs sont les suivants :" +
                          "\n Etat de la case : {plateau[ligne, colonne].etat}" +
                          "\n Etait-il en feu ? : {plateau[ligne, colonne].etaitFeu}");
    }
    

    #endregion
    
    
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Cellule[,] plateau = RemplissageAleatoire();
        
        //int tours = SaisieSecu();
        Affichage(plateau);
        Console.ReadKey();
        InitialiseFeu(plateau);
        Affichage(plateau);
        while (Console.ReadKey().Key != ConsoleKey.Q)
        {
            Simulation(plateau);
            
        }
        
        

    }
}