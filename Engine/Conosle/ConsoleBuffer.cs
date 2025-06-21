using System.IO;
using GameInConsole.Engine;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SDL2;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using StbImageSharp;
using StbImageWriteSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Processing;

namespace GameInConsoleEngine.Engine
{
    public class PixelInfo
    {
        public Vector2i Point;
        public Rgba32 foreGroundColor;
        public Rgba32 backGroundColor;
        public char character;
    }
    class ConsoleBuffer
    {
        string tempImgFile;
        readonly int width, height;

        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
             1.0f,  1.0f, 0.0f, 1.0f, 1.0f, // top right
             1.0f, -1.0f, 0.0f, 1.0f, 0.0f, // bottom right
            -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, // bottom left
            -1.0f,  1.0f, 0.0f, 0.0f, 1.0f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _elementBufferObject;

        private int _vertexBufferObject;

        private int _vertexArrayObject;

        private Shader _shader;

        private int _texture;

        public ConsoleBuffer(int w, int he)
        {
            //tempImgFile = Path.GetTempPath() + "image.bmp";
            tempImgFile = "./temp.png";
            width = w;
            height = he;

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            _shader = new Shader("./Engine/Shaders/shader.vert", "./Engine/Shaders/shader.frag");
            _shader.Use();

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));


            _texture = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _texture);

            // float[] borderColor = { 1.0f, 1.0f, 0.0f, 1.0f };
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        /// <summary>
        /// Sets the buffer to values
        /// </summary>
        /// <param name="GlyphBuffer"></param>
        /// <param name="charBuffer"> array of chars which get added to the buffer</param>
        /// <param name="colorBuffer"> array of foreground(front)colors which get added to the buffer</param>
        /// <param name="background"> array of background colors which get added to the buffer</param>
        /// <param name="defualtBackground"> default color(may reduce fps?), this is the background color
        ///									null chars will get set to this default background</param>
        public void SetBuffer(Glyph[,] GlyphBuffer, Rgba32 defualtBackground)
        {
            List<byte> data = new List<byte>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Rgba32 rgba32 = ((Rgba32)GlyphBuffer[x, y]);
                    data.AddRange(new byte[] { rgba32.R, rgba32.G, rgba32.B, 255 });
                }
            }

            Image<Rgba32> imageResult = Image.LoadPixelData<Rgba32>(data.ToArray(), width, height);
            ImageResult image = null;
            StbImage.stbi_set_flip_vertically_on_load(1);
            using (FileStream stream = new FileStream(tempImgFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                imageResult.SaveAsPng(stream, new PngEncoder()
                {
                    ColorType = PngColorType.RgbWithAlpha,
                });
            }

            using (FileStream stream = new FileStream(tempImgFile, FileMode.OpenOrCreate, FileAccess.Read))
            {
                image = ImageResult.FromStream(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
            }


            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _texture);
        }

        public void Blit(GameWindow window, Rgba32 backGroundColor)
        {
            DateTime dateTimeNow = DateTime.Now;
            // SDL.SDL_RenderClear(renderer);
            
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BindVertexArray(_vertexArrayObject);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _texture);

            _shader.Use();

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            window.SwapBuffers();
  
            double diff = (DateTime.Now - dateTimeNow).TotalMilliseconds;
        }
        public void Exit()
        {
            File.Delete(tempImgFile);
        }
    }
}
