using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Media;
using SFML.Learning;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;


namespace double_card
{
    internal class Program:Game
    {
        static int[,] cards;
        static int[] imageID;
        static string[] iconName;
        static int[,] choiseLevel;

        static int width_card = 100;
        static int height_card = 100;
        static int top_offset = 80;
        static int left_offset = 50;
        static int space = 50;

        static int left_menuOffset = 250;
        static int width_choise = 300;
        static int height_choise = 100;

        static int count_cards;
        static int count_choise = 3;
        static int count_per_line = 5;
        static int id = 0;

        static string ruleGround = LoadTexture("rule.png");
        static string choiseGround = LoadTexture("menu_1.png");
            
        static string background = LoadTexture("background.png");
        static string background_song= LoadMusic("background.wav");
        static string openCard_song= LoadSound("open.wav");
        static string openedCard_song= LoadSound("true.wav"); 
        static string closedCard_song= LoadSound("error.wav"); 

        static bool isgame= false;
        static void StartMenu()
        {
            DrawSprite(background, 0, 0);
            DrawSprite(ruleGround, 90, 300);
            SetFillColor(Color.Black);
            DrawText(140, 320,  "           Правила игры: ",30);
            DrawText(140, 350,  "- Выберите уровень сложности ",28);
            DrawText(140, 375,  "   с помощью клавиш Num1/2/3",28);
            DrawText(140, 470, "- Управляйте с помощью ЛКМ", 28);
            DrawText(140, 500, "- Следите за таймером",28);
            DrawText(140, 410, "- Постарайтесь запомнить карты",28);
            DrawText(140, 440, "- Найдите пару для каждой из них",28);
            DrawText(140, 540, "           Желаю удачи!",30);

            choiseLevel = new int[count_choise,4];

            for (int i = 0; i < choiseLevel.GetLength(0); i++)
            {
                choiseLevel[i, 0] = ((i % 1) * (width_choise + space) + left_menuOffset);  //pos x                      
                choiseLevel[i, 1] = ((i / 1) * ( space*2) );  //pos y              
                choiseLevel[i, 2] = width_choise ; //weidth sprite
                choiseLevel[i, 3] = height_choise ; //height sprite
                
            }
            for (int i = 0; i < choiseLevel.GetLength(0); i++)
            {
                DrawSprite(choiseGround, choiseLevel[i, 0], choiseLevel[i, 1]);

                DrawText(choiseLevel[0, 0]+100, choiseLevel[0, 1]+30, "1.Легкий", 28);
                DrawText(choiseLevel[1, 0]+100, choiseLevel[1, 1]+30, "2.Средний", 28);
                DrawText(choiseLevel[2, 0]+100, choiseLevel[2, 1]+30, "3.Тяжелый", 28);
            }
        }
        static void InitChoise()
        {
            if (GetKey(Keyboard.Key.Num1) == true)
            {
                count_cards = 8;
                isgame = true;
            }
            if (GetKey(Keyboard.Key.Num2) == true)
            {
                count_cards = 14;
                isgame = true;
            }
            if (GetKey(Keyboard.Key.Num3) == true)
            {
                count_cards = 20;
                isgame = true;
            }
        }

        static void LoadIcons()
        {
            iconName = new string[8];

            iconName[0] = LoadTexture("icon_0.png");

            for (int i = 1; i < iconName.Length; i++)
            {
                iconName[i] = LoadTexture("icon_" + (i).ToString() + ".png");
            }
        }
        static void Shuffle(int[] mix)
        {
            Random rand = new Random();

            for (int i = mix.Length-1; i >= 0; i--)
            {
                int j=rand.Next(1,i+1);
                int tpm = mix[j];
                mix[j] = mix[i];
                mix[i] = tpm;
            }   
        }
        static void InitID()
        {
            Random rnd=new Random();

            imageID = new int[count_cards];

            for(int i = 0; i < imageID.Length; i++)
            {
                if (i % 2 == 0)
                {
                    id = rnd.Next(1, 8);
                }
                imageID[i] = id;               
            }

            Shuffle(imageID);
            Shuffle(imageID);
        }       
        static void InitCard()
        {                  
            InitID();

            cards = new int[count_cards, 6];

            for(int i = 0; i <cards.GetLength(0) ; i++)
            {
                cards[i, 0] = 1; //state
                cards[i, 1] = ((i % count_per_line) * (width_card + space) + left_offset);  //pos x
                cards[i, 2] = ((i / count_per_line) * (height_card + space) + top_offset);  //pos y
                cards[i, 3] = width_card;  //width
                cards[i, 4] = height_card;  //height
                cards[i, 5] = imageID[i];  //identify 
            }                          
        }
        static void DrawCard()
        {
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                if (cards[i, 0] == 1) //open
                {
                    DrawSprite(iconName[cards[i, 5]], cards[i, 1], cards[i, 2]);
                }

                if (cards[i, 0] == 0) //close
                {
                    DrawSprite(iconName[0], cards[i, 1], cards[i, 2]);
                }              
            }
        }
        static int GetIndexCardByMousePosition()
        {
   
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                if (MouseX >= cards[i, 1] && MouseX <= cards[i, 1] + cards[i, 3] && MouseY >= cards[i, 2] && MouseY <= cards[i, 2] + cards[i, 4])
                {
                    return i;
                }
            }
            return -1;
        }
        static void SetVisibilityForCards(int state)
        {
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                cards[i, 0] = state;
            }
        }
        static void Main(string[] args)
        {
            int openCardAmount = 0;
            int firstOpenCardIndex = -1;
            int secondOpenCardIndex = -1;
            int swatch = 0;
            int time = 0;

            SetFont("Andre_V.ttf");

            InitWindow(800, 700, "Double card");

            while (isgame == false)
            {
                DispatchEvents();

                InitChoise();// выбор сложности (1-easy;2-medium;3-hard)   


                StartMenu();// отрисовка старт меню

                DisplayWindow();

                Delay(1);
            }

            int remainingCard = count_cards;

            if (isgame == true)
            {
                LoadIcons();
                InitCard();

                DrawSprite(background, 0, 0);
                PlayMusic(background_song, 50);

                SetVisibilityForCards(1);

                DrawCard();
                DisplayWindow();
                Delay(2000);

                SetVisibilityForCards(0);

                while (remainingCard != 0)
                {
                    DispatchEvents();

                    if (openCardAmount == 2)
                    {
                        if (cards[firstOpenCardIndex, 5] == cards[secondOpenCardIndex, 5])
                        {
                            PlaySound(openedCard_song, 70);

                            cards[firstOpenCardIndex, 0] = -1;
                            cards[secondOpenCardIndex, 0] = -1;

                            remainingCard -= 2;
                        }
                        if (cards[firstOpenCardIndex, 5] != cards[secondOpenCardIndex, 5])
                        {
                            PlaySound(closedCard_song, 100);

                            cards[firstOpenCardIndex, 0] = 0;
                            cards[secondOpenCardIndex, 0] = 0;
                        }

                        firstOpenCardIndex = -1;
                        secondOpenCardIndex = -1;

                        openCardAmount = 0;

                        Delay(1500);
                    }

                    if (GetMouseButtonDown(0) == true)
                    {
                        int index = GetIndexCardByMousePosition();

                        if (index != -1 && index != firstOpenCardIndex)
                        {
                            PlaySound(openCard_song, 80);

                            cards[index, 0] = 1;

                            openCardAmount++;

                            if (openCardAmount == 1) firstOpenCardIndex = index;
                            if (openCardAmount == 2) secondOpenCardIndex = index;
                        }
                    }
                    swatch++;
                    time = (3600 - swatch) / 60;

                    ClearWindow();

                    DrawSprite(background, 0, 0);
                    SetFillColor(Color.White);
                    DrawText(10, 10, "Времени осталось: "+ time, 20);
                    DrawCard();

                    DisplayWindow();

                    Delay(1);

                    if (remainingCard == 0) break;
                    if (swatch == 3600) break;
                }
                if (remainingCard == 0)
                {
                    ClearWindow();

                    DrawSprite(background, 0, 0);
                    SetFillColor(Color.White);
                    DrawText(160, 270, "Поздравляю, вы открыли все карты! ", 30);
                    DisplayWindow();
                    Delay(3500);
                }
                if (swatch == 3600)
                {
                    ClearWindow();

                    DrawSprite(background, 0, 0);
                    SetFillColor(Color.White);
                    DrawText(200, 270, "У вас закончилось время!", 30);
                    DisplayWindow();
                    Delay(3500);
                }
            }          
        }
    }
}
