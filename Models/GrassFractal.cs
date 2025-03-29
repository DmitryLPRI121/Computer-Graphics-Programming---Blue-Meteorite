using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class GrassFractal : TransformableObject, IDisposable
    {
        private int VBO, VAO, IBO;
        private Texture texture;
        private List<float> vertices = new List<float>();
        private List<uint> indices = new List<uint>();

        private int depth; // глубина рекурсии для фрактала
        private float baseHeight; // базовая высота стебля
        private float baseWidth; // базовая ширина стебля
        private float randomFactor; // фактор случайности для естественности
        private int grassCount; // количество травинок

        public GrassFractal(string texturePath, int depth = 3, float baseHeight = 1000.4f, float baseWidth = 0.05f, float randomFactor = 0.3f, int grassCount = 100)
        {
            this.depth = depth;
            this.baseHeight = baseHeight;
            this.baseWidth = baseWidth;
            this.randomFactor = randomFactor;
            this.grassCount = grassCount;

            GenerateFractalGrass();
            InitializeBuffers();
            texture = new Texture(texturePath);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void GenerateFractalGrass()
        {
            Random random = new Random();
            uint indexOffset = 0;

            for (int i = 0; i < grassCount; i++)
            {
                // Случайная позиция травинки по X и Z со значительно увеличенным диапазоном
                float posX = (float)(random.NextDouble() * 40 - 20); // От -20 до 20
                float posZ = (float)(random.NextDouble() * 40 - 20); // От -20 до 20

                // Случайные вариации высоты и ширины для каждой травинки
                float height = baseHeight * (0.7f + (float)random.NextDouble() * randomFactor);
                float width = baseWidth * (0.8f + (float)random.NextDouble() * randomFactor * 0.5f);

                // Генерируем одну травинку
                GenerateGrassBlade(posX, 0, posZ, height, width, depth, random, ref indexOffset);
            }
        }

        private void GenerateGrassBlade(float x, float y, float z, float height, float width, int currentDepth, Random random, ref uint indexOffset)
        {
            if (currentDepth <= 0)
                return;

            // Базовый зеленый цвет с небольшой вариацией
            float r = 0.1f + (float)random.NextDouble() * 0.2f;
            float g = 0.5f + (float)random.NextDouble() * 0.3f;
            float b = 0.1f + (float)random.NextDouble() * 0.1f;

            // Случайное отклонение для верхней точки с фрактальным узором
            float bendFactor = 0.2f - (currentDepth * 0.03f); // Уменьшенный изгиб
            float bendX = (float)(random.NextDouble() * 2 - 1) * randomFactor * bendFactor;
            float bendZ = (float)(random.NextDouble() * 2 - 1) * randomFactor * bendFactor;

            // Фрактальные вариации высоты и ширины
            float fractalNoise = (float)Math.Sin(x * 5 + z * 3) * 0.1f + (float)Math.Cos(x * 2 - z * 7) * 0.1f;
            height *= (0.8f + fractalNoise * 0.5f); // Меньше вариация высоты
            width *= (1 - fractalNoise * 0.3f);

            // Создаем сегмент травы (прямоугольник)
            float halfWidth = width / 2;
            
            // Нижние вершины
            // Позиция                   Нормаль         Текст.коорд.
            vertices.Add(x - halfWidth); vertices.Add(y); vertices.Add(z);
            vertices.Add(0); vertices.Add(1); vertices.Add(0);
            vertices.Add(0); vertices.Add(0);
            
            vertices.Add(x + halfWidth); vertices.Add(y); vertices.Add(z);
            vertices.Add(0); vertices.Add(1); vertices.Add(0);
            vertices.Add(1); vertices.Add(0);

            // Верхние вершины - изгибаются в случайном направлении
            vertices.Add(x + halfWidth + bendX); vertices.Add(y + height); vertices.Add(z + bendZ);
            vertices.Add(0); vertices.Add(1); vertices.Add(0);
            vertices.Add(1); vertices.Add(1);
            
            vertices.Add(x - halfWidth + bendX); vertices.Add(y + height); vertices.Add(z + bendZ);
            vertices.Add(0); vertices.Add(1); vertices.Add(0);
            vertices.Add(0); vertices.Add(1);

            // Индексы для образования двух треугольников (прямоугольника)
            indices.Add(indexOffset);
            indices.Add(indexOffset + 1);
            indices.Add(indexOffset + 2);

            indices.Add(indexOffset);
            indices.Add(indexOffset + 2);
            indices.Add(indexOffset + 3);

            indexOffset += 4; // 4 вершины добавлено

            // Если глубина рекурсии больше 1, добавляем ветвления с фрактальным паттерном
            if (currentDepth > 1)
            {
                // Для большей глубины мы добавляем ветки, начинающиеся от верхней части текущего сегмента
                float newHeight = height * (0.5f + fractalNoise * 0.15f); // Уменьшение высоты для веток
                float newWidth = width * (0.6f - fractalNoise * 0.1f); // Уменьшение ширины для веток
                
                // Точка ветвления - верхняя центральная точка текущего сегмента
                float branchX = x + bendX;
                float branchY = y + height;
                float branchZ = z + bendZ;

                // Количество ветвлений зависит от глубины рекурсии
                int branches = currentDepth == depth ? 3 : 2;
                
                // Добавляем несколько ветвлений
                for (int i = 0; i < branches; i++)
                {
                    float angleFactor = (float)i / branches * MathHelper.TwoPi + (float)random.NextDouble() * 0.5f;
                    float lateralOffset = (float)Math.Sin(angleFactor) * width * 0.5f;
                    float forwardOffset = (float)Math.Cos(angleFactor) * width * 0.5f;
                    
                    // Фрактальное ветвление с вариациями параметров
                    GenerateGrassBlade(
                        branchX + lateralOffset,
                        branchY,
                        branchZ + forwardOffset,
                        newHeight * (1 + (float)random.NextDouble() * 0.1f),
                        newWidth * (1 - (float)random.NextDouble() * 0.1f),
                        currentDepth - 1,
                        random,
                        ref indexOffset
                    );
                }
            }
        }

        private void InitializeBuffers()
        {
            if (vertices.Count == 0 || indices.Count == 0)
                return;

            // Преобразуем списки в массивы для OpenGL
            float[] vertexArray = vertices.ToArray();
            uint[] indexArray = indices.ToArray();

            // Создаем и заполняем VBO, VAO и IBO
            GL.GenBuffers(1, out VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexArray.Length * sizeof(float), vertexArray, BufferUsageHint.StaticDraw);

            GL.GenVertexArrays(1, out VAO);
            GL.BindVertexArray(VAO);

            // Позиция
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            // Нормаль
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            // Текстурные координаты
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            // Создаем буфер индексов
            GL.GenBuffers(1, out IBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexArray.Length * sizeof(uint), indexArray, BufferUsageHint.StaticDraw);
        }

        protected override void Draw(Shader shader)
        {
            // Привязываем VAO (который автоматически привязывает буфер индексов IBO)
            GL.BindVertexArray(VAO);

            // Активируем текстуру
            texture.Use(TextureUnit.Texture0);

            // Используем DrawElements для отрисовки с индексами
            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);

            // Отключаем привязку VAO для безопасности
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            // Освобождаем OpenGL ресурсы
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(IBO);
            GL.DeleteVertexArray(VAO);
        }
    }
} 