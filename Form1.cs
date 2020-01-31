using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace TiPPGK2
{
    public partial class Form1 : Form
    {
        private readonly Graphics _graphics;
        private Bitmap _bmp = null;

        AnimatedSprite spaceShip = new AnimatedSprite();
        
        List<AnimatedSprite> rockets = new List<AnimatedSprite>();
        AnimData rocketsData = new AnimData();

        Sprite background = new Sprite();
        Sprite spaceStation1 = new Sprite();
        Sprite spaceStation2 = new Sprite();

        public Form1()
        {
            InitializeComponent();

            _bmp = new Bitmap(1280, 720);
            _graphics = Graphics.FromImage(_bmp);
            pictureBox1.Image = _bmp;

            /*
            ######################################
                          Spaceship
            ######################################
            */
            spaceShip.animData.images = new Image[24];
            spaceShip.animData.frameInfo = new AnimFrameData[24];

            for (int i = 0; i < 24; i += 3)
            {
                spaceShip.animData.images[i] = TiPPGK2.Properties.Resources.spaceship1;
                spaceShip.animData.images[i + 1] = TiPPGK2.Properties.Resources.spaceship2;
                spaceShip.animData.images[i + 2] = TiPPGK2.Properties.Resources.spaceship3;

                spaceShip.animData.frameInfo[i].startFrame = i;
                spaceShip.animData.frameInfo[i].numFrames = 23;

                spaceShip.animData.frameInfo[i + 1].startFrame = i + 1;
                spaceShip.animData.frameInfo[i + 1].numFrames = 23;

                spaceShip.animData.frameInfo[i + 2].startFrame = i + 2;
                spaceShip.animData.frameInfo[i + 2].numFrames = 23;
            }

            spaceShip.Initialize(spaceShip.animData, 0);
            spaceShip.x = 620;
            spaceShip.y = 620;

            /*
            ######################################
                           Rockets
            ######################################
            */

            rocketsData.images = new Image[7];
            rocketsData.images[0] = TiPPGK2.Properties.Resources.explosion1;
            rocketsData.images[1] = TiPPGK2.Properties.Resources.explosion2;
            rocketsData.images[2] = TiPPGK2.Properties.Resources.explosion3;
            rocketsData.images[3] = TiPPGK2.Properties.Resources.explosion4;
            rocketsData.images[4] = TiPPGK2.Properties.Resources.explosion5;
            rocketsData.images[5] = TiPPGK2.Properties.Resources.explosion6;
            rocketsData.images[6] = TiPPGK2.Properties.Resources.explosion7;

            rocketsData.frameInfo = new AnimFrameData[7];
            for (int i = 0; i < 7; i++)
            {
                rocketsData.frameInfo[i].startFrame = i;
                rocketsData.frameInfo[i].numFrames = 6;
            }

            /*
            ######################################
                   Background + spacestations
            ######################################
            */
            background.image = TiPPGK2.Properties.Resources.space;
            background.y = -background.image.Height + 720;

            spaceStation1.image = TiPPGK2.Properties.Resources.starbase;
            spaceStation1.x = 130;
            spaceStation1.y = -256;

            spaceStation2.image = TiPPGK2.Properties.Resources.starbase;
            spaceStation2.x = 850;
            spaceStation2.y = -700;

            var timer = new Timer();

            timer.Interval = 16;
            timer.Tick += OnTick;
            timer.Start();
            Invalidate();
        }

        private void OnTick(object sender, EventArgs e)
        {
            _graphics.Clear(Color.Aqua);

            using (Graphics g = Graphics.FromImage(_bmp))
            {
                background.Draw(g, background.image.Width, background.image.Height);
                if (background.y != 0)
                    background.y += 1;
                else
                    background.y = -background.image.Height + 720;

                spaceStation1.Draw(g, 256, 256);
                if (spaceStation1.y < 820)
                    spaceStation1.y += 5;
                else
                    spaceStation1.y = -256;

                spaceStation2.Draw(g, 352, 352);
                if (spaceStation2.y < 820)
                    spaceStation2.y += 3;
                else
                    spaceStation2.y = -256;

                spaceShip.Draw(g);
                spaceShip.UpdateAnim(timer1.Interval / 1000.0f);

                foreach (AnimatedSprite sprites in rockets)
                {
                    if (IsRocket(sprites))
                        sprites.Draw(g, 64, 32);
                    else
                        sprites.Draw(g);

                    if (sprites.y == 0)
                    {
                        sprites.UpdateAnim(timer1.Interval / 1000.0f);
                        if (sprites.frameNum == 5)
                            sprites.image = null;
                    }
                    else
                    {
                        sprites.y -= 5;
                    }
                }

                rockets.RemoveAll(IsNull);

                Refresh();
                Invalidate();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                spaceShip.Move(sender, e, 10);
            else if (e.KeyCode == Keys.Space)
            {
                rockets.Add(new AnimatedSprite());
                rockets.Last().Initialize(rocketsData, 0);
                rockets.Last().image = TiPPGK2.Properties.Resources.rocket;
                rockets.Last().x = spaceShip.x - 10;
                rockets.Last().y = spaceShip.y - 20;

                rockets.Add(new AnimatedSprite());
                rockets.Last().Initialize(rocketsData, 0);
                rockets.Last().image = TiPPGK2.Properties.Resources.rocket;
                rockets.Last().x = spaceShip.x + 10;
                rockets.Last().y = spaceShip.y - 20;
            }

        }

        public bool IsNull(AnimatedSprite sprite)
        {
            if (sprite.image == null)
                return true;
            return false;
        }

        public bool IsRocket(AnimatedSprite sprite)
        {
            if (sprite.image.Size == TiPPGK2.Properties.Resources.rocket.Size)
                return true;
            return false;
        }
    }

    public struct AnimFrameData
    {
        public int startFrame;
        public int numFrames;
    }

    public struct AnimData
    {
        public Image[] images;
        public AnimFrameData[] frameInfo;
    }

    public class Sprite
    {
        public Image image;
        public int drawImage;
        public int x, y;

        public void Draw(Graphics g, int width = 64, int height = 80)
        {
            g.DrawImage(image, x, y, width, height);
        }
    }

    public class AnimatedSprite : Sprite
    {
        public AnimData animData;
        public int animNum;
        public int frameNum;
        public float frameTime;
        public float animFPS = 24.0f;

        public void Initialize(AnimData myData, int startingAnimNum)
        {
            animData = myData;
            ChangeAnim(startingAnimNum);
        }

        public void ChangeAnim(int num)
        {
            animNum = num;
            frameNum = 0;
            frameTime = 0.0f;
            int imageNum = animData.frameInfo[animNum].startFrame;
            image = animData.images[imageNum];
        }

        public void UpdateAnim(float deltaTime)
        {
            frameTime += deltaTime;
            if (frameTime > 1 / animFPS)
            {
                frameNum += (int)(frameTime * animFPS);
                if (frameNum >= animData.frameInfo[animNum].numFrames)
                {
                    frameNum = frameNum % animData.frameInfo[frameNum].numFrames;
                }
                
                int imageNum = animData.frameInfo[animNum].startFrame + frameNum;
                image = animData.images[imageNum];
                frameTime = frameTime % (1 / animFPS);
            }
        }

        public void Move(object sender, KeyEventArgs e, int speed)
        {
            if (e.KeyCode == Keys.Left && x > 0)
                x -= speed;
            else if (e.KeyCode == Keys.Right && x < 1210)
                x += speed;
            else if (e.KeyCode == Keys.Up && y > 0)
                y -= speed;
            else if (e.KeyCode == Keys.Down && y < 640)
                y += speed;
        }
    }
}
