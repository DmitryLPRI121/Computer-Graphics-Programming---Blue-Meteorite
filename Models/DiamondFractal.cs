using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class DiamondFractal : TransformableObject, IDisposable
    {
        private int VBO, VAO, IBO;
        private Texture texture;
        private List<float> vertices = new List<float>();
        private List<uint> indices = new List<uint>();

        private int depth; // глубина рекурсии для фрактала
        private float baseSize; // базовый размер ромба
        private float randomFactor; // фактор случайности для естественности
        private int diamondCount; // количество начальных ромбов
        private Vector3 initialColor; // начальный цвет ромба

        public DiamondFractal(string texturePath, int depth = 3, float baseSize = 1.0f, float randomFactor = 0.3f, int diamondCount = 10, Vector3? initialColor = null, float repeatCount = 1.0f)
        {
            this.depth = depth;
            this.baseSize = baseSize;
            this.randomFactor = randomFactor;
            this.diamondCount = diamondCount;
            this.initialColor = initialColor ?? new Vector3(0.7f, 0.3f, 0.9f); // Фиолетовый цвет по умолчанию

            GenerateFractalDiamonds();
            InitializeBuffers();
            
            // Если путь к текстуре не указан или файл не существует, используем встроенную текстуру
            if (string.IsNullOrEmpty(texturePath))
            {
                texture = new Texture("textures/meteorite.jpg", repeatCount); // Используем существующую текстуру
            }
            else
            {
                texture = new Texture(texturePath, repeatCount);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void GenerateFractalDiamonds()
        {
            Random random = new Random();
            uint indexOffset = 0;

            if (diamondCount == 1)
            {
                // Если нужен только один ромб, генерируем его в центре
                GenerateDiamond(0, 0, 0, baseSize * 2.0f, depth, random, ref indexOffset);
                return;
            }

            for (int i = 0; i < diamondCount; i++)
            {
                // Случайная позиция ромба по X и Z
                float posX = (float)(random.NextDouble() * 40 - 20); // От -20 до 20
                float posZ = (float)(random.NextDouble() * 40 - 20); // От -20 до 20
                float posY = (float)(random.NextDouble() * 2); // Немного случайности по Y

                // Случайные вариации размера для каждого ромба
                float size = baseSize * (0.7f + (float)random.NextDouble() * randomFactor);

                // Генерируем один ромб
                GenerateDiamond(posX, posY, posZ, size, depth, random, ref indexOffset);
            }
        }

        private void GenerateDiamond(float x, float y, float z, float size, int currentDepth, Random random, ref uint indexOffset)
        {
            if (currentDepth <= 0)
                return;

            // Вычисляем цвет на основе глубины рекурсии
            float depthFactor = (float)currentDepth / depth;
            Vector3 color = new Vector3(
                initialColor.X * depthFactor,
                initialColor.Y * depthFactor + (1 - depthFactor) * 0.3f,
                initialColor.Z * depthFactor + (1 - depthFactor) * 0.2f
            );

            // Вершины ромба
            float halfSize = size / 2;
            
            // Создаем вершины ромба (4 основных вершины)
            // Вершинные точки ромба:
            // 0: верхняя точка
            // 1: правая точка
            // 2: нижняя точка
            // 3: левая точка

            // Случайные отклонения для естественности
            float randomTop = (float)(random.NextDouble() * randomFactor * size * 0.2f);
            float randomRight = (float)(random.NextDouble() * randomFactor * size * 0.2f);
            float randomBottom = (float)(random.NextDouble() * randomFactor * size * 0.2f);
            float randomLeft = (float)(random.NextDouble() * randomFactor * size * 0.2f);

            // Верхняя точка
            vertices.Add(x); vertices.Add(y + halfSize + randomTop); vertices.Add(z);
            // Нормаль (направлена вверх для верхней точки)
            vertices.Add(0); vertices.Add(1); vertices.Add(0);
            // Текстурные координаты
            vertices.Add(0.5f); vertices.Add(1.0f);

            // Правая точка
            vertices.Add(x + halfSize + randomRight); vertices.Add(y); vertices.Add(z);
            // Нормаль (направлена вправо для правой точки)
            vertices.Add(1); vertices.Add(0); vertices.Add(0);
            // Текстурные координаты
            vertices.Add(1.0f); vertices.Add(0.5f);

            // Нижняя точка
            vertices.Add(x); vertices.Add(y - halfSize - randomBottom); vertices.Add(z);
            // Нормаль (направлена вниз для нижней точки)
            vertices.Add(0); vertices.Add(-1); vertices.Add(0);
            // Текстурные координаты
            vertices.Add(0.5f); vertices.Add(0.0f);

            // Левая точка
            vertices.Add(x - halfSize - randomLeft); vertices.Add(y); vertices.Add(z);
            // Нормаль (направлена влево для левой точки)
            vertices.Add(-1); vertices.Add(0); vertices.Add(0);
            // Текстурные координаты
            vertices.Add(0.0f); vertices.Add(0.5f);

            // Добавляем еще 1 точку в центре для создания 3D формы
            vertices.Add(x); vertices.Add(y); vertices.Add(z + halfSize);
            // Нормаль (направлена к наблюдателю)
            vertices.Add(0); vertices.Add(0); vertices.Add(1);
            // Текстурные координаты
            vertices.Add(0.5f); vertices.Add(0.5f);

            // Добавляем точку сзади для создания 3D формы
            vertices.Add(x); vertices.Add(y); vertices.Add(z - halfSize);
            // Нормаль (направлена от наблюдателя)
            vertices.Add(0); vertices.Add(0); vertices.Add(-1);
            // Текстурные координаты
            vertices.Add(0.5f); vertices.Add(0.5f);

            // Определим индексы для создания граней (треугольников)
            // Передняя сторона
            // Верхняя грань (верх-право-центр)
            indices.Add(indexOffset);
            indices.Add(indexOffset + 1);
            indices.Add(indexOffset + 4);

            // Правая грань (право-низ-центр)
            indices.Add(indexOffset + 1);
            indices.Add(indexOffset + 2);
            indices.Add(indexOffset + 4);

            // Нижняя грань (низ-лево-центр)
            indices.Add(indexOffset + 2);
            indices.Add(indexOffset + 3);
            indices.Add(indexOffset + 4);

            // Левая грань (лево-верх-центр)
            indices.Add(indexOffset + 3);
            indices.Add(indexOffset + 0);
            indices.Add(indexOffset + 4);

            // Задняя сторона
            // Верхняя грань (верх-право-центр)
            indices.Add(indexOffset);
            indices.Add(indexOffset + 1);
            indices.Add(indexOffset + 5);

            // Правая грань (право-низ-центр)
            indices.Add(indexOffset + 1);
            indices.Add(indexOffset + 2);
            indices.Add(indexOffset + 5);

            // Нижняя грань (низ-лево-центр)
            indices.Add(indexOffset + 2);
            indices.Add(indexOffset + 3);
            indices.Add(indexOffset + 5);

            // Левая грань (лево-верх-центр)
            indices.Add(indexOffset + 3);
            indices.Add(indexOffset + 0);
            indices.Add(indexOffset + 5);

            // Боковые грани
            // Верхняя боковая грань
            indices.Add(indexOffset + 0);
            indices.Add(indexOffset + 4);
            indices.Add(indexOffset + 5);

            // Правая боковая грань
            indices.Add(indexOffset + 1);
            indices.Add(indexOffset + 4);
            indices.Add(indexOffset + 5);

            // Нижняя боковая грань
            indices.Add(indexOffset + 2);
            indices.Add(indexOffset + 4);
            indices.Add(indexOffset + 5);

            // Левая боковая грань
            indices.Add(indexOffset + 3);
            indices.Add(indexOffset + 4);
            indices.Add(indexOffset + 5);

            indexOffset += 6; // 6 вершин добавлено

            // Если глубина рекурсии больше 1, добавляем подфракталы
            if (currentDepth > 1)
            {
                // Размер для подфракталов
                float newSize = size * 0.5f;
                
                // Фрактальный шум для вариаций
                float fractalNoise = (float)Math.Sin(x * 5 + z * 3) * 0.1f + (float)Math.Cos(x * 2 - z * 7) * 0.1f;
                
                // Создаем 6 подфракталов (по одному для каждой вершины)
                float offset = size * 0.7f; // Отступ для размещения подфракталов
                
                // 1. Верхний подфрактал
                GenerateDiamond(
                    x, 
                    y + offset, 
                    z,
                    newSize * (1 + fractalNoise * 0.2f),
                    currentDepth - 1,
                    random,
                    ref indexOffset
                );
                
                // 2. Правый подфрактал
                GenerateDiamond(
                    x + offset, 
                    y, 
                    z,
                    newSize * (1 + fractalNoise * 0.1f),
                    currentDepth - 1,
                    random,
                    ref indexOffset
                );
                
                // 3. Нижний подфрактал
                GenerateDiamond(
                    x, 
                    y - offset, 
                    z,
                    newSize * (1 - fractalNoise * 0.1f),
                    currentDepth - 1,
                    random,
                    ref indexOffset
                );
                
                // 4. Левый подфрактал
                GenerateDiamond(
                    x - offset, 
                    y, 
                    z,
                    newSize * (1 - fractalNoise * 0.2f),
                    currentDepth - 1,
                    random,
                    ref indexOffset
                );

                // 5. Передний подфрактал
                GenerateDiamond(
                    x, 
                    y, 
                    z + offset,
                    newSize * (1 + fractalNoise * 0.15f),
                    currentDepth - 1,
                    random,
                    ref indexOffset
                );

                // 6. Задний подфрактал
                GenerateDiamond(
                    x, 
                    y, 
                    z - offset,
                    newSize * (1 - fractalNoise * 0.15f),
                    currentDepth - 1,
                    random,
                    ref indexOffset
                );
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