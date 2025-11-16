namespace Синхронизация
{
    internal class Program
    {
        static Properties properties;

        static void Main(string[] args)
        {
            properties = new Properties($"{Environment.CurrentDirectory}\\properties.json");

            if (args.Length > 0)
            {
                Property prop = properties.propertiesList[int.Parse(args[0])];

                SyncProject(prop);
            } else
            {
                bool flag = true;
                
                while (flag)
                {
                    Draw();

                    var k = Console.ReadKey();
                    Console.WriteLine();

                    switch (k.KeyChar)
                    {
                        case '+':
                            AddProperty();
                            break;

                        case '-':
                            DeleteProperty();
                            break;
                        case 'g':
                            AskSync();
                            break;
                        case 'l':
                            CreateLink();
                            break;
                        case 'q':
                            flag = false;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        static void Draw()
        {
            Console.Clear();
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("                   СПИСОК СИНХРОНИЗАЦИЙ");
            Console.WriteLine("------------------------------------------------------------");
            for (int i = 0; i < properties.propertiesList.Count; i++)
            {
                Property prop = properties.propertiesList[i];
                Console.WriteLine($"Номер синхронизации: {i + 1}\nНазвание: {prop.name}\nОписание: {prop.description}\nПуть главной папки (источник): \"{prop.pathSourse}\"\nПуть синхронизации: \"{prop.pathCopy}\"\nИмя флешки: \"{prop.fleshName}\"");
                Console.WriteLine("--------------------------------------------------------");
            }
            Console.WriteLine("Список горячих клавиш:\n\"+\" - добавление новой синхронизации\n\"-\" - удаление синхронизации\n\"g\" - запустить синхронизацию\n\"l\" - создать ярлык синхронизации\n\n\"q\" - выход");
        }

        static void AddProperty()
        {
            Property property = new Property();
            Console.Write("Введите имя синхронизации: ");
            property.name = Console.ReadLine();
            Console.Write("Введите описание синхронизации: ");
            property.description = Console.ReadLine();
            Console.Write("Введите путь до главной папки: ");
            property.pathSourse = Console.ReadLine();
            Console.Write("Введите путь для синхронизации: ");
            property.pathCopy = Console.ReadLine();
            Console.Write("Введите имя флешки: ");
            property.fleshName = Console.ReadLine();

            properties.AddProperty(property);
        }

        static void DeleteProperty()
        {
            Console.Write("Введите номер синхронизации для удаления: ");
            int index = int.Parse(Console.ReadLine()) - 1;

            properties.DeleteProperty(index);
        }

        static void AskSync()
        {
            Console.Write("Введите номер синхронизации для запуска: ");
            int index = int.Parse(Console.ReadLine()) - 1;
            Console.Clear();
            Property prop = properties.propertiesList[index];

            SyncProject(prop);

            Console.WriteLine("для перехода в главное меню нажмите любую клавишу");
            Console.ReadKey();
        }

        static void CreateLink()
        {
            Console.Write("Введите номер синхронизации: ");
            int index = int.Parse(Console.ReadLine()) - 1;
            Property prop = properties.propertiesList[index];
            Console.Write("Введите путь для ярлыка: ");
            string name = Console.ReadLine();

            FileHandler.createLinkFile(name, prop.name, Environment.ProcessPath, index.ToString());
        }

        static void SyncProject(Property prop)
        {
            try
            {
                Sync sync = new Sync(prop.pathSourse, prop.pathCopy, Console.WriteLine, properties.notCopiedKeys);
                sync.Copy();
                sync.CheckFilesAndDirs();

                Console.WriteLine("------------------");
                Console.WriteLine("ПРОВЕРКА ЗАВЕРШЕНА");
                Console.WriteLine("------------------");
                Console.WriteLine();

                bool success = UsbEject.EjectDrive(prop.fleshName);
                if (success)
                {
                    Console.WriteLine("Флешка успешно извлечена");
                }
                else
                {
                    Console.WriteLine("Ошибка при извлечении флешки");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
