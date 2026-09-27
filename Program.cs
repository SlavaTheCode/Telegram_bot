using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    // Словарь для хранения состояния пользователя: текущий режим
    static ConcurrentDictionary<long, UserState> userStates = new ConcurrentDictionary<long, UserState>();

    // ID общего чата, куда будут отправляться все сообщения
    static long targetChatId = -4811543268; // замените на свой ID чата
    static string token = "[TOP SECRET]"; // вставьте свой токен
    static string[] Article_list = new string[80] 
    {
        "Лучший городской транспорт - Трамвай\n \n https://telegra.ph/Luchshij-gorodskoj-transport--Tramvaj-06-25 \n",
        "Роль озеленения в развитии современных городов\n \n https://telegra.ph/Rol-ozeleneniya-v-razvitii-sovremennyh-gorodov-07-05 \n",
        "Загранпаспорт - что это такое и с чем его не едят?\n \n https://telegra.ph/Zagranpasport-pasport--chto-ehto-takoe-i-s-chem-ego-ne-edyat-07-06 \n",
        "Как отправить поезд только на одном рельсе?\n \n https://telegra.ph/Kak-otpravit-poezd-tolko-na-odnom-relse-07-12 \n",
        "Президентские выборы в США\n \n https://telegra.ph/Prezidentskie-vybory-v-SSHA-07-12 \n",
        "Сохраняя и преобразуя здания в России\n \n https://telegra.ph/Sohranyaya-i-preobrazuya-zdaniya-v-Rossii-07-15 \n",
        "Иммиграция в Германию\n \n https://telegra.ph/Immigraciya-v-Germaniyu-07-26 \n",
        "Равные права граждан современной России, или коротко о феминизме\n \n https://telegra.ph/Ravnye-prava-grazhdan-sovremennoj-Rossii-ili-korotko-o-feminizme-07-26 \n",
        "Важность общественного и личного транспорта\n \n https://telegra.ph/Vazhnost-obshchestvennogo-i-lichnogo-transporta-07-28 \n",
        "Психология толпы и её влияние на индивидуальное поведение\n \n https://telegra.ph/Psihologiya-tolpy-i-ee-vliyanie-na-individualnoe-povedenie-08-17 \n",
        "Как и зачем реклама влияет на вас?\n \n https://telegra.ph/Kak-i-zachem-reklama-vliyaet-na-vas-09-01 \n",
        "Рекламные стратегии в индустрии книготорговли\n \n https://telegra.ph/Reklamnye-strategii-v-industrii-knigotorgovli-09-03 \n",
        "Хороша ли революция?\n \n https://telegra.ph/Horosha-li-revolyuciya-09-14 \n",
        "Самопринуждение и его влияние на личность\n \n https://telegra.ph/Samoprinuzhdenie-i-ego-vliyanie-na-lichnost-09-18 \n",
        "Как умер федерализм в России?\n \n https://telegra.ph/Kak-umer-federalizm-v-Rossii-A-Respublika-gosudarstvo-Bashkortostan-utratila-suverenitet-09-22 \n",
        "«Ноги в руки и вперёд». Что мешает человеку вечно заниматься работой или любимым делом?\n \n https://telegra.ph/Nogi-v-ruki-i-vperyod-CHto-meshaet-cheloveku-vechno-zanimatsya-rabotoj-ili-lyubimym-uvlecheniem-09-30 \n",
        "Праздничная атмосфера и её влияние на городскую среду\n \n https://telegra.ph/Prazdnichnaya-atmosfera-i-eyo-vliyanie-na-gorodskuyu-sredu-12-22-2 \n",
        "Новый год: Что это и как его отмечают?\n \n https://telegra.ph/Novyj-god-CHto-ehto-i-kak-ego-otmechayut-12-29 \n",
        "Города агломерации\n \n https://telegra.ph/Goroda-aglomeracii-01-01 \n",
        "Что же такое Лига Наций?\n \n https://telegra.ph/Liga-nacij-01-08 \n",
        "21 число каждого месяца - День писем политзаключенным\n \n https://telegra.ph/21-chislo-kazhdogo-mesyaca---Den-pisem-politzaklyuchennym-01-19 \n",
        "Искусство и спорт времён перестройки\n \n https://telegra.ph/Iskusstvo-i-sport-vremyon-perestrojki-01-22 \n",
        "Стадии уголовного преследования\n \n https://telegra.ph/Stadii-ugolovnogo-presledovaniya-01-27 \n",
        "Августовский путч: вся необходимая информация (и красивые фотографии)\n \n https://telegra.ph/Avgustovskij-putch-vsya-neobhodimaya-informaciya-i-krasivye-fotografii-01-30 \n",
        "Мишши Сеспел: Из двух зол\n \n https://telegra.ph/MISHSHI-%D2%AAE%D2%AAP%D3%96L-IZ-DVUH-ZOL-02-05 \n",
        "История правозащитников в СССР\n \n https://telegra.ph/Istoriya-pravozashchitnikov-v-SSSR-02-09 \n",
        "Урбанистика: коротко и ясно\n \n https://telegra.ph/Urbanistika-korotko-i-yasno-02-09 \n",
        "Обороняющаяся демократия: чего?\n \n https://telegra.ph/Oboronyayushchayasya-demokratiya-chego-02-19 \n",
        "Что такое социал-демократия? \n\n https://telegra.ph/CHto-takoe-social-demokratiya-03-05 \n",
        "Нейросети: Революция в области искусственного интеллекта\n \n https://telegra.ph/Nejroseti-Revolyuciya-v-oblasti-iskusstvennogo-intellekta-03-09 \n",
        "Реагенты против гололёда: победители и проигравшие\n \n https://telegra.ph/Reagenty-protiv-gololyoda-pobediteli-i-proigravshie-03-12 \n",
        "От Ruthenia до Первых: долгая история студенческих объединений\n \n https://telegra.ph/Ot-Ruthenia-do-Pervyh-dolgaya-doroga-studencheskih-obedinenij-03-19 \n",
        "Взаимосвязь индекса демократии и коррупции\n \n https://telegra.ph/Vzaimosvyaz-indeksa-demokratii-i-urovnya-korrupcii-03-31 \n",
        "ГУЛАГ - часть первая\n \n https://telegra.ph/GULAG---chast-pervaya-04-06 \n",
        "Световая проблема\n \n https://telegra.ph/Svetovaya-problema-04-10 \n",
        "Судебное разбирательство\n \n https://telegra.ph/Sudebnoe-razbiratelstvo-04-13 \n",
        "История повторяется\n \n https://telegra.ph/Istoriya-povtoryaetsya-04-25 \n",
        "Оксигейт и реакция на него\n \n https://telegra.ph/Oksigejt-i-reakciya-na-nego-05-02\n",
        "Что-то про Танк(!)\n \n https://telegra.ph/CHto-to-pro-Tank-05-08 \n",
        "Логические ошибки: разбираемся в их видах\n \n https://telegra.ph/Logicheskie-oshibki-razbiraemsya-v-ih-vidah-05-16 \n",
        "Общественное радиовещание - что это такое?\n \n https://telegra.ph/Obshchestvennoe-radioveshchanie--chto-ehto-takoe-05-17 \n",
        "Домашнее насилие: почему это не нормально\n \n https://telegra.ph/Domashnee-nasilie-pochemu-ehto-ne-normalno-05-21 \n",
        "Парядок преследования за административные правонарушения\n \n https://telegra.ph/Poryadok-presledovaniya-za-administrativnye-pravonarusheniya-05-28 \n", //
        "Все время перед глазами, но вы не замечаете\n \n https://telegra.ph/My-vse-ravny-06-04 \n",
        "ЮНЕСКО: как культура стала ценностью\n \n https://telegra.ph/YUNESKO-kak-kultura-stala-cennostyu-06-08 \n",
        "Новый взгляд на высоту: можно строить многоэтажки и не превращать город в муравейник?\n \n https://telegra.ph/Novyj-vzglyad-na-vysotu-06-11 \n",
        "«Мост» к искусству\n \n https://telegra.ph/Most-k-iskusstvu-06-15 \n",
        "Груминг — это не только про собачек\n \n https://telegra.ph/Gruming-pochemu-vazhno-ob-ehtom-znat-06-16 \n",
        "«Отец Канады» - Пьер Элиот Трюдо\n \n https://telegra.ph/Otec-Kanady---Per-EHliot-Tryudo-06-17 \n",
        "Уличные животные: опасность или трагедия?\n \n https://telegra.ph/Ulichnye-zhivotnye-opasnost-ili-tragediya-06-17\n",
        "Кажется, мы все всемирно запутались\n \n https://telegra.ph/Istoriya-World-Wide-Web-stanovlenie-i-vozmozhnye-puti-razvitiya-07-02 \n",
        "Меджусловјанскы језык? Што?\n \n https://telegra.ph/Mezhslavyanskij-CHto-ehto-za-yazyk-07-04 \n",
        "Изящная фея русского балета\n \n https://telegra.ph/Izyashchnaya-feya-russkogo-baleta--Ekaterina-Maksimova-07-06 \n",
        "Мегапроектирование\n \n https://telegra.ph/Mega-proekty-v-SSSR-07-09 \n",
        "Через репрессии к легенде русского балета\n \n https://telegra.ph/Velikaya-Majya-Pliseckaya-07-14 \n",
        "Фашисты будущего будут называть себя антифашистами\n \n https://telegra.ph/Fashisty-budushchego-budut-nazyvat-sebya-antifashistami-07-21 \n",
        "Агриппина Яковлевна Ваганова — царица вариаций\n \n https://telegra.ph/Agrippina-YAkovlevna-Vaganova--carica-variacij-07-31 \n",
        "Коровы — главные виновники глобального потепления?\n \n https://telegra.ph/Korovy--glavnye-vinovniki-globalnogo-potepleniya-08-04 \n",
        "Альтернативные источники энергии: лучше ли традиционных?\n \n https://telegra.ph/Alternativnye-istochniki-ehlektroehnergii-luchshe-li-tradicionnyh-08-10 \n",
        "Литр воды, макароны, соль и святой дух, готовить на медленном огне 15 минут и вы получаете очень вкусную религию!\n \n https://telegra.ph/Pastafarianstvo--makarony-kak-mirovaya-religiya-08-14 \n",
        "Это лето было слишком жарким.. Опять?\n \n https://telegra.ph/GLOBALNOE-POTEPLENIE-I-EGO-VLIYANIE-NA-NAS-08-18 \n",
        "Искусство, эмиграция и помощь детям\n \n https://telegra.ph/Zagadochnaya-balerina--Anna-Pavlova-08-21 \n",
        "История гражданского неповиновения: от Антигоны до наших дней \n \n https://telegra.ph/Istoriya-grazhdanskogo-nepovinoveniya-ot-Antigony-do-nashih-dnej-09-04 \n",
        "Македонцы и где они обитают \n \n https://telegra.ph/Istoriya-Makedonii-v-tryoh-aktah-09-07 \n",
        "Последний из нас \n \n https://telegra.ph/Poslednij-iz-nas-09-12 \n",
        "История глазами кота \n \n https://telegra.ph/O-sebe-i-mire-vokrug-glazami-kota-09-15 \n",
        "Учение об обществе \n \n https://telegra.ph/Klassiki-sociologii-i-ih-idei-10-07 \n",
        "От интервенции до спасения миллионов \n \n https://telegra.ph/Ot-intervencii-do-spaseniya-millionov-10-17 \n",
        "Пришёл, увидел, институт \n \n https://telegra.ph/Prishyol-uvidel-institut-Cikly-zhizni-obshchestvennyh-dvizhenij-10-23 \n",
        "Невада-Семей \n \n https://telegra.ph/Rech-ne-stavshaya-predvybornoj-11-07 \n",
        "Молекула, которая создала государство \n \n https://telegra.ph/Molekula-kotoraya-sozdala-gosudarstvo-kak-rech-Haima-Vejcmana-podarila-evreyam-Izrail-11-10 \n",
        "Полезно ли сексуальное просвещение? \n \n https://telegra.ph/Polezno-li-seksualnoe-prosveshchenie-11-10 \n",
        "Много навыков или один на полную мощность? \n \n https://telegra.ph/Mnogo-navykov-ili-odin-na-polnuyu-moshchnost-11-18 \n",
        "Четыре мира, один вектор: типы (роли) активистов в общественных движениях \n \n https://telegra.ph/CHetyre-mira-odin-vektor-tipy-roli-aktivistov-v-obshchestvennyh-dvizheniyah-11-21 \n",
        "Международные отношения в истории \n \n https://telegra.ph/Evropejskie-mezhdunarodnye-otnoshenie-v-XVII-veke-01-08 \n",
        "Юмор как способ спасения \n \n https://telegra.ph/Tragikomediya-01-21 \n",
        "Гренландия раздора \n \n https://telegra.ph/Holodilnik-razdora-ili-Grenlandiya-protiv-vseh-01-30 \n",
        "История Гринвичской обсерватории \n \n https://telegra.ph/Grinvichskaya-observatoriya-01-30 \n",
        "Когда учёба — по-настоящему боль \n \n https://telegra.ph/EHksperiment-Milgrehma-02-06 \n",
        "Как менять систему без революций? \n \n https://telegra.ph/Upravlencheskaya-filosofiya-kajdzen-sut-i-vozmozhnosti-dlya-grazhdanskogo-obshchestva-02-13 \n"
    };
    public static class FotoState
    {
        public static bool TooMuchFoto = false;
    }

    static async Task Main()
    {
        var botClient = new TelegramBotClient(token);

        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token);

        Console.WriteLine("Бот запущен");
        Console.WriteLine("Чтобы выйти, нажмите Enter.");
        // Ожидание бесконечно долго
        await Task.Delay(-1);
        cts.Cancel();
    }

    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message == null)
            return;

        var message = update.Message;
        long chatId = message.Chat.Id;
        var fileId = "BAACAgIAAxkBAAIEdWmUkEfvyJ9tvoKlNYVxzmN7M6Q_AAJYmgACzgqpSJoyMmTLAl-TOgQ";

        // Получаем или создаем состояние пользователя
        var userState = userStates.GetOrAdd(chatId, new UserState());

        if (message.Text != null || (message.Photo != null || message.Video != null) && message.Caption != null)
        {
            string text = " ";
            if (message.Text != null) { text = message.Text.Trim(); }
            if (message.Caption != null) { text = message.Caption.Trim();  }
            if (chatId != targetChatId)
            {
                if (text == "/start")
                {
                    await botClient.SendMessage(chatId, "Добро пожаловать в бот для связи с Академией! Можете использовать следующие команды -\n" +
                        " \n" +
                        "/info - Информация о нашей организации\n" +
                        "/read - Случайная статья в Академии\n" +
                        "/join - Вступить в Академию\n" +
                        "/media - Наши социальные сети\n" +
                        "/send - Написать нам сообщение на любую тему\n" +
                        "/idea - Предложить тему для нашего следующего материала (поста/статьи)\n" +
                        "/team - Предложить нам сотрудничество\n" +
                        "/report - Сообщить нам об ошибке\n" +
                        " \n" +
                        "/Roskomnadzor - небольшое напоминание об этой конторе\n" +
                        " \n" +
                        "/menu - Отменить прошлое действие и вернуться в меню\n" +
                        " \n" +
                        "Спасибо, что пользуетесь нашим ботом!", cancellationToken: cancellationToken);
                    FotoState.TooMuchFoto = false;
                    return;
                }
                // Обработка команд
                else if (text == "/info")
                {
                    await botClient.SendMessage(chatId, "Молодёжная Академия — это образовательная организация, созданная для формирования прагматичной точки зрения, популяризации науки и для повышения осведомлённости граждан о нашем мире.\n" +
                        " \n" +
                        "Наша команда ставит своей целью повышение грамотности среди граждан и особенно молодёжи, создавая наши материалы в более простом, удобном и неформальном стиле.", cancellationToken: cancellationToken);
                    FotoState.TooMuchFoto = false;
                    return;
                }
                else if (text == "/read")
                {
                    Random random = new Random();
                    await botClient.SendMessage(chatId, "Наша случайная статья для вас —\n" +
                        " \n" +
                        Article_list[random.Next(0,Article_list.Length)] +
                        " \n" +
                        "Спасибо, что читаете нас и пользуетесь нашим ботом. Приятного чтения!", cancellationToken: cancellationToken); ;
                    FotoState.TooMuchFoto = false;
                    return;
                }
                else if (text == "/join")
                {
                    await botClient.SendMessage(chatId, "Спасибо, что решили вступить в Академию! Чтобы вступить, заполните анкету по форме —\n" +
                        " \n" +
                        "https://forms.gle/PDtw5YW8MnVA7UW6A \n" +
                        " \n" +
                        "На данный момент это единственный способ вступить. Если у вас возникли трудности с вступлением или заполнением анкеты, можете написать нам /report.", cancellationToken: cancellationToken);
                    FotoState.TooMuchFoto = false;
                    return;
                }
                else if (text == "/media")
                {
                    await botClient.SendMessage(chatId, "Наши социальные сети —\n" +
                        " \n" +
                        "Telegram - https://t.me/MolAcademy \n" +
                        " \n" +
                        "Tiktok - https://www.tiktok.com/@molacademyofficially?_t=ZS-8wUud0fjg5x&_r=1 \n" +
                        " \n" +
                        "На данный момент это все наши активные социальные сети.", cancellationToken: cancellationToken);
                    FotoState.TooMuchFoto = false;
                    return;
                }
                else if (text == "/send")
                {
                    userState.CurrentMode = Mode.SendMessage;
                    await botClient.SendMessage(chatId, "Если вы хотите нам что-то сообщить, рассказать или посоветовать - можете написать ваше сообщение ниже. Пожалуйста, не отправляйте больше одного фото/видео за раз, мы получим только первое.\n" +
                        " \n" +
                        "Учитывайте, что мы не сможем ответить вам здесь. Если хотите, чтобы мы написали вам в ответ, не забудьте оставить свой @тег.", cancellationToken: cancellationToken);
                    FotoState.TooMuchFoto = false;
                    return;
                }
                else if (text == "/idea")
                {
                    userState.CurrentMode = Mode.SendIdea;
                    await botClient.SendMessage(chatId, "Если у вас есть тема для нашего следующего материала(поста/статьи), которую вы хотите предложить, или вы хотели бы больше наших материалов на определённую тематику - можете написать ваше сообщение ниже. Пожалуйста, не отправляйте больше одного фото/видео за раз, мы получим только первое.\n" +
                        " \n" +
                        "Учитывайте, что мы не сможем ответить вам здесь. Если хотите, чтобы мы написали вам в ответ, не забудьте оставить свой @тег.", cancellationToken: cancellationToken);
                    FotoState.TooMuchFoto = false;
                    return;
                }
                else if (text == "/team")
                {
                    userState.CurrentMode = Mode.SendTeam;
                    await botClient.SendMessage(chatId, "Если вы хотите предложить нам сотрудничество, будучи представителем проекта/медиа/организации, просьба написать:\n" +
                        " \n" +
                        "1. Название вашего проекта/медиа/организации;\n" +
                        "2. Ссылка на все/основной ваш информационный ресурс;\n" +
                        "3. Описание вашего проета/медиа/организации;\n" +
                        "4. Ваш @тег в телеграм или номер для связи с вами;\n" +
                        " \n" +
                        "Спасибо, что выбрали сотрудничество с нами! Учитывайте, что мы не сможем ответить вам здесь.", cancellationToken: cancellationToken);
                    FotoState.TooMuchFoto = false;
                    return;
                }
                else if (text == "/report")
                {
                    userState.CurrentMode = Mode.SendReport;
                    await botClient.SendMessage(chatId, "Если вы хотите нам сообщить нам об ошибке - можете написать ваше сообщение ниже. Пожалуйста, не отправляйте больше одного фото/видео за раз, мы получим только первое.\n" +
                        " \n" +
                        "Учитывайте, что мы не сможем ответить вам здесь. Если хотите, чтобы мы написали вам в ответ, не забудьте оставить свой @тег.", cancellationToken: cancellationToken);
                    FotoState.TooMuchFoto = false;
                    return;
                }
                else if (text == "/Roskomnadzor")
                {
                    await botClient.SendVideo(
                    chatId: chatId,
                    video: fileId,
                    caption: "Напоминание о том, что Роскомнадзор нехорошие люди.");
                    FotoState.TooMuchFoto = false;
                    return;
                }
                else if (text == "/menu")
                {
                    await botClient.SendMessage(chatId, "Вы вернулись в меню. Можете использовать следующие команды —\n" +
                " \n" +
                "/info - Информация о нашей организации \n" +
                "/read - Случайная статья в Академии \n" +
                "/join - Вступить в Академию \n" +
                "/media - Наши социальные сети \n" +
                "/send - Написать нам сообщение на любую тему \n" +
                "/idea - Предложить тему для нашего следующего материала (поста/статьи)\n" +
                "/team - Предложить нам сотрудничество\n" +
                "/report - Сообщить нам об ошибке\n" +
                " \n" +
                "/Roskomnadzor - небольшое напоминание об этой конторе\n" +
                " \n" +
                "/menu - Отменить прошлое действие и вернуться в меню\n" +
                " \n" +
                "Спасибо, что пользуетесь нашим ботом!", cancellationToken: cancellationToken);
                    FotoState.TooMuchFoto = false;
                }
                // Обработка сообщений в режиме
                else
                {
                    switch (userState.CurrentMode)
                    {
                        case Mode.None:
                            await botClient.SendMessage(chatId, "Вы не выбрали команду или ошиблись в ней. Возможно, вы отправили стикер или голосовое сообщение - мы их не принимаем. Используйте команду /menu чтобы вернуться в меню и просмотреть доступные команды.\n" +
                        " \n" +
                        "Спасибо, что пользуетесь нашим ботом!", cancellationToken: cancellationToken);
                            break;
                        case Mode.SendMessage:
                            await ForwardAndAcknowledge(botClient, message, "#сообщение", cancellationToken);
                            userState.CurrentMode = Mode.None;
                            break;
                        case Mode.SendIdea:
                            await ForwardAndAcknowledge(botClient, message, "#тема", cancellationToken);
                            userState.CurrentMode = Mode.None;
                            break;
                        case Mode.SendTeam:
                            await ForwardAndAcknowledge(botClient, message, "#сотрудничество", cancellationToken);
                            userState.CurrentMode = Mode.None;
                            break;
                        case Mode.SendReport:
                            await ForwardAndAcknowledge(botClient, message, "#ошибка", cancellationToken);
                            userState.CurrentMode = Mode.None;
                            break;
                        default:
                            await botClient.SendMessage(chatId, "Вы не выбрали команду или ошиблись в ней. Возможно, вы отправили фото/видео без подписи, стикер или голосовое сообщение - мы их не принимаем. Если вы отправили больше одного за раз, мы получили только первое. Используйте команду /menu чтобы вернуться в меню и просмотреть доступные команды.\n" +
                            " \n" +
                            "Спасибо, что пользуетесь нашим ботом!", cancellationToken: cancellationToken);
                            break;
                    }
                }
            }
        }
        else if ((message.Photo != null || message.Video != null) && message.Caption == null && FotoState.TooMuchFoto == false)
        {
            await botClient.SendMessage(chatId, "Вы отправили фото/видео без подписи - мы их не принимаем. Если вы отправили больше одного за раз, мы получили только первое. Используйте команду /menu чтобы вернуться в меню и просмотреть доступные команды.\n" +
            " \n" +
            "Спасибо, что пользуетесь нашим ботом!", cancellationToken: cancellationToken);
            FotoState.TooMuchFoto = true;
        }
        else if (((message.Photo != null || message.Video != null) && message.Caption == null) && FotoState.TooMuchFoto == true) { } 
    }

    static async Task ForwardAndAcknowledge(ITelegramBotClient botClient, Message message, string signatureTag, CancellationToken token)
    {
        // Пересылаем сообщение в целевой чат
        await botClient.ForwardMessage(targetChatId, message.Chat.Id, message.MessageId);

        // Отправляем подпись
        await botClient.SendMessage(targetChatId, signatureTag);

        // Отвечаем пользователю что сообщение отправлено
        await botClient.SendMessage(message.Chat.Id, "Мы получили ваше сообщение. Помните, что мы не сможем ответить вам здесь. Спасибо, что пользуетесь нашим ботом!", cancellationToken: token);
    }

    static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
    {
        var errorMsg = exception switch
        {
            ApiRequestException apiEx => $"Ошибка API:\n[{apiEx.ErrorCode}]\n{apiEx.Message}",
            _ => exception.ToString()
        };
        Console.WriteLine(errorMsg);
        return Task.CompletedTask;
    }

    class UserState
    {
        public Mode CurrentMode { get; set; } = Mode.None;
    }

    enum Mode
    {
        None,
        SendReport,
        SendMessage,
        SendIdea,
        SendTeam
    }
}
