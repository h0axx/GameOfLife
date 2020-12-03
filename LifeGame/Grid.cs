using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LifeGame
{
    class Grid : DrawableGameComponent
    {
        const int GRID_WIDTH = 400;
        const int GRID_HEIGHT = 200;
        const int UPDATE_INTERVAL = 33;

        bool[,,] cells;
        Rectangle[,] rects;

        SpriteBatch spriteBatch;
        Texture2D livingCellTexture;
        Input input;

        public Grid(Game game, SpriteBatch spriteBatch, Input input) : base(game)
        {
            cells = new bool[2, GRID_WIDTH + 2, GRID_HEIGHT + 2];
            rects = new Rectangle[GRID_WIDTH, GRID_HEIGHT];
            this.spriteBatch = spriteBatch;
            this.input = input;
        }

        protected override void LoadContent()
        {

            livingCellTexture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] colors = new Color[1];
            colors[0] = Color.White;

            livingCellTexture.SetData(0, new Rectangle(0, 0, 1, 1), colors, 0, 1);

            base.LoadContent();
        }

        int millisecondsSinceLastUpdate = 0;
        int currentIndex = 0, futureIndex = 1;

        int oldWidth, oldHeight;

        public override void Update(GameTime gameTime)
        {
            if (input.SpaceTrigger)
            {
                createRandomCells(7);
            }

            #region GameLogic
            millisecondsSinceLastUpdate += gameTime.ElapsedGameTime.Milliseconds;

            if (millisecondsSinceLastUpdate >= UPDATE_INTERVAL)
            {
                millisecondsSinceLastUpdate = 0;

                for (int y = 1; y < GRID_HEIGHT; y++)
                {
                    for (int x = 1; x < GRID_WIDTH; x++)
                    {
                        int neighborsCount = 0;

                        //Left
                        if(cells[currentIndex, x - 1, y] == true)
                        {
                            neighborsCount++;
                        }
                        //Right
                        if (cells[currentIndex, x + 1, y] == true)
                        {
                            neighborsCount++;
                        }
                        //Over
                        if (cells[currentIndex, x, y - 1] == true)
                        {
                            neighborsCount++;
                        }
                        //Under
                        if (cells[currentIndex, x, y + 1] == true)
                        {
                            neighborsCount++;
                        }
                        //Left-Over
                        if (cells[currentIndex, x - 1, y - 1] == true)
                        {
                            neighborsCount++;
                        }
                        //Right-Over
                        if (cells[currentIndex, x + 1, y - 1] == true)
                        {
                            neighborsCount++;
                        }
                        //Left-Under
                        if (cells[currentIndex, x - 1, y + 1] == true)
                        {
                            neighborsCount++;
                        }
                        //Right-Under
                        if (cells[currentIndex, x + 1, y + 1] == true)
                        {
                            neighborsCount++;
                        }
                        //Put result into next generation grid
                        if (neighborsCount == 3)
                        {
                            cells[futureIndex, x, y] = true;
                        }
                        else if (neighborsCount == 2 && cells[currentIndex, x, y])
                        {
                            cells[futureIndex, x, y] = true;
                        }
                        else
                        {
                            cells[futureIndex, x, y] = false;
                        }
                    }
                }

                if (currentIndex == 0)
                {
                    currentIndex = 1;
                    futureIndex = 0;
                }
                else
                {
                    currentIndex = 0;
                    futureIndex = 1;
                }
            }
            #endregion

            #region Grid

            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            if (oldWidth != width || oldHeight != height)
            {

                int cellWidth = width / GRID_WIDTH;
                int cellHeight = height / GRID_HEIGHT;

                int cellSize = Math.Min(cellWidth, cellHeight);

                int offsetX = (width - (cellSize * GRID_WIDTH)) / 2;
                int offsetY = (height - (cellSize * GRID_HEIGHT)) / 2;

                for (int y = 0; y < GRID_HEIGHT; y++)
                {
                    for (int x = 0; x < GRID_WIDTH; x++)
                    {
                        rects[x, y] = new Rectangle(offsetX + x * cellSize, offsetY + y * cellSize, cellSize, cellSize);
                    }
                }

                oldHeight = height;
                oldWidth = width;
            }

            #endregion

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin();

            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    if(cells[currentIndex, x + 1, y + 1])
                    {
                        spriteBatch.Draw(livingCellTexture, rects[x, y], Color.White);
                    }
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }


        private void createRandomCells(int probability)
        {

            Random r = new Random();

            for (int x = 1; x < GRID_WIDTH; x++)
            {
                for (int y = 1; y < GRID_HEIGHT; y++)
                {
                    if (r.Next(0, probability) == 0)
                    {
                        cells[currentIndex, x, y] = true;
                    }
                    else
                    {
                        cells[currentIndex, x, y] = false;
                    }
                }
            }
        }
    }

}
