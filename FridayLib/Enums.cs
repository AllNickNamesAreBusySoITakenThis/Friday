using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib
{
    public enum PPOCategories
    {
        [Description("Уставки")]
        Ustavki,
        [Description("Печать и выгрузка файлов")]
        PrintingExporting,
        [Description("Тренды")]
        Trends,
        [Description("Регистрация и отображение событий")]
        Events,
        [Description("Конфигурирование ПО")]
        Config,
        [Description("Резервное копирование")]
        Backup,
        [Description("Управление доступом")]
        Access,
        [Description("Служебные библиотеки")]
        Sevice,
        [Description("Связь и синхронизация")]
        Connection,
        [Description("Контроль целостности")]
        FileControl,
        [Description("Иное")]
        Other
    }
    public enum PPOTasks
    {
        [Description("SCADA. Дополнение функционала")]
        SCADA_Addons,
        [Description("ОС. Дополнение функционала")]
        OS_Addons,
        [Description("Информационная безопасность")]
        IB
    }
    public enum PPOReestrStatus
    {
        [Description("Проверка не требуется")]
        TestsNotNeeded,
        [Description("Не проверено")]
        NotTested,
        [Description("Проверяется")]
        OnTesting,
        [Description("Прошла проверку")]
        PassTests,
        [Description("Не прошла проверку")]
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
