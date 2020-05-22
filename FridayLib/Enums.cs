using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib
{
    public static class EnumHelper
    {
        public static string Description(this Enum value)
        {
            var attributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Any())
                return (attributes.First() as DescriptionAttribute).Description;
            // If no description is found, the least we can do is replace underscores with spaces
            // You can add your own custom default formatting logic here
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(ti.ToLower(value.ToString().Replace("_", " ")));
        }
        public static IEnumerable<ValueDescription> GetAllValuesAndDescriptions(Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException($"{nameof(t)} must be an enum type");
            return Enum.GetValues(t).Cast<Enum>().Select((e) => new ValueDescription() { Value = e, Description = e.Description() }).ToList();
        }
    }

    public class ValueDescription
    {
        public Enum Value { get; internal set; }
        public string Description { get; internal set; }
    }

    public enum PPOCategories
    {
        [Description("Уставки")]
        [Display(Name = "Уставки", Description = "Уставки")]
        Ustavki,
        [Description("Печать и выгрузка файлов")]
        [Display(Name = "Печать и выгрузка файлов", Description = "Печать и выгрузка файлов")]
        PrintingExporting,
        [Description("Тренды")]
        [Display(Name = "Тренды", Description = "Тренды")]
        Trends,
        [Description("Регистрация и отображение событий")]
        [Display(Name = "Регистрация и отображение событий", Description = "Регистрация и отображение событий")]
        Events,
        [Description("Конфигурирование ПО")]
        [Display(Name = "Конфигурирование ПО", Description = "Конфигурирование ПО")]
        Config,
        [Description("Резервное копирование")]
        [Display(Name = "Резервное копирование", Description = "Резервное копирование")]
        Backup,
        [Description("Управление доступом")]
        [Display(Name = "Управление доступом", Description = "Управление доступом")]
        Access,
        [Description("Служебные библиотеки")]
        [Display(Name = "Служебные библиотеки", Description = "Служебные библиотеки")]
        Sevice,
        [Description("Связь и синхронизация")]
        [Display(Name = "Связь и синхронизация", Description = "Связь и синхронизация")]
        Connection,
        [Description("Контроль целостности")]
        [Display(Name = "Контроль целостности", Description = "Контроль целостности")]
        FileControl,
        [Description("Иное")]
        [Display(Name = "Иное", Description = "Иное")]
        Other
    }
    public enum PPOTasks
    {
        //[Description("ППО, созданное с целью реализации функций, дополняющих базовый функционал SCADA-систем")]
        [Display(Name = "Дополнение SCADA", Description = "ППО, созданное с целью реализации функций, дополняющих базовый функционал SCADA-систем")]
        SCADA_Addons,
        [Description("ОС. Дополнение функционала")]
        [Display(Name = "Дополнение ОС", Description = "ОС. Дополнение функционала")]
        OS_Addons,
        [Description("Информационная безопасность")]
        [Display(Name = "Информационная безопасность", Description = "Информационная безопасность")]
        IB
    }
    public enum PPOReestrStatus
    {
        [Display(Name = "Проверка не требуется", Description = "Приложение не требуется регистрировать в реестре ППО")]
        TestsNotNeeded,
        [Display(Name = "Проверка не проводилась", Description = "Приложение не отправлялось на проверку в реестр ППО")]
        NotTested,
        [Display(Name = "Находится на проверке", Description = "Приложение находится на проверке в реестре ППО")]
        OnTesting,
        [Display(Name = "Прошел проверку", Description = "Приложение находится в реестре ППО")]
        PassTests,
        [Display(Name = "Провалил проверку", Description = "Приложение не не находится в реестре ППО")]
        FailTests
    }
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
