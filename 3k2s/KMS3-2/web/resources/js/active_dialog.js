var dialogOn = false;
var recognition = null;
var speechOn = false;

var endings = [
    ["ет", "(ет|ут|ют)"],
    ["ит", "(ит|ат|ят)"],
    ["ает", "(ает|ают)"],
    ["яет", "(яет|яют)"],
    ["ется", "(ется|ются)"],
    ["ится", "(ится|ятся)"],
    ["ен", "(ен|ена|ено|ены)"],
    ["ан", "(ан|ана|ано|аны)"],
    ["является", "(является|являются)"],
    ["используется", "(используется|используются)"],
    ["предназначен", "(предназначен|предназначена|предназначено|предназначены)"],
    ["состоит", "(состоит|состоят)"],
    ["имеет", "(имеет|имеют)"],
    ["показывает", "(показывает|показывают)"],
    ["содержит", "(содержит|содержат)"],
    ["включает", "(включает|включают)"],
    ["выглядит", "(выглядит|выглядят)"]
];

function small1(str) {
    return str.substring(0, 1).toLowerCase() + str.substring(1);
}

function big1(str) {
    return str.substring(0, 1).toUpperCase() + str.substring(1);
}

function getEnding(word) {
    for (var i = 0; i < endings.length; i++) {
        if (word.endsWith(endings[i][0])) {
            return word.substring(0, word.length - endings[i][0].length) + endings[i][1];
        }
    }

    return "";
}

function getAnswer(question) {
    function normalize(text) {
        return text
            .toLowerCase()
            .replaceAll("ё", "е")
            .replaceAll("?", " ")
            .replaceAll(".", " ")
            .replaceAll(",", " ")
            .replaceAll("!", " ")
            .replace(/\s+/g, " ")
            .trim()
            .replaceAll(" ", "");
    }

    var questionText = question
        .toLowerCase()
        .replaceAll("ё", "е")
        .replaceAll("?", " ")
        .replaceAll(".", " ")
        .replaceAll(",", " ")
        .replaceAll("!", " ")
        .replace(/\s+/g, " ")
        .trim();

    var originalQuestion = questionText;
    var clearQuestion = questionText;

    var neededPredicate = "";

    if (originalQuestion.indexOf("как выглядит") != -1) {
        neededPredicate = "выгляд";
    }

    if (originalQuestion.indexOf("из чего состоит") != -1) {
        neededPredicate = "состоит|содержит|включает";
    }

    if (originalQuestion.indexOf("что содержит") != -1) {
        neededPredicate = "содержит|состоит|включает|имеет";
    }

    if (originalQuestion.indexOf("что показывает") != -1) {
        neededPredicate = "показывает";
    }

    if (
        originalQuestion.indexOf("зачем") != -1 ||
        originalQuestion.indexOf("для чего") != -1
    ) {
        neededPredicate = "предназначен|предназначена|используется|нужен|нужна|помогает|обеспечивает";
    }

    if (originalQuestion.indexOf("как работает") != -1) {
        neededPredicate = "работает|движется|управляется|передает|создает|обеспечивает";
    }

    if (
        originalQuestion.indexOf("кто создал") != -1 ||
        originalQuestion.indexOf("кто разработал") != -1
    ) {
        neededPredicate = "создан|разрабатывался";
    }

    if (
        originalQuestion.indexOf("где создан") != -1 ||
        originalQuestion.indexOf("где создали") != -1 ||
        originalQuestion.indexOf("где производится") != -1 ||
        originalQuestion.indexOf("где выпускается") != -1
    ) {
        neededPredicate = "производится|выпускается";
    }

    clearQuestion = clearQuestion.replace("что такое", "");
    clearQuestion = clearQuestion.replace("кто такой", "");
    clearQuestion = clearQuestion.replace("кто такая", "");
    clearQuestion = clearQuestion.replace("кто создал", "");
    clearQuestion = clearQuestion.replace("кто разработал", "");
    clearQuestion = clearQuestion.replace("что представляет собой", "");
    clearQuestion = clearQuestion.replace("для чего нужен", "");
    clearQuestion = clearQuestion.replace("для чего нужна", "");
    clearQuestion = clearQuestion.replace("для чего нужно", "");
    clearQuestion = clearQuestion.replace("для чего нужны", "");
    clearQuestion = clearQuestion.replace("зачем нужен", "");
    clearQuestion = clearQuestion.replace("зачем нужна", "");
    clearQuestion = clearQuestion.replace("зачем нужно", "");
    clearQuestion = clearQuestion.replace("зачем нужны", "");
    clearQuestion = clearQuestion.replace("из чего состоит", "");
    clearQuestion = clearQuestion.replace("что показывает", "");
    clearQuestion = clearQuestion.replace("что содержит", "");
    clearQuestion = clearQuestion.replace("что делает", "");
    clearQuestion = clearQuestion.replace("как выглядит", "");
    clearQuestion = clearQuestion.replace("как работает", "");
    clearQuestion = clearQuestion.replace("где находится", "");
    clearQuestion = clearQuestion.replace("где расположен", "");
    clearQuestion = clearQuestion.replace("где расположена", "");
    clearQuestion = clearQuestion.replace("где располагается", "");
    clearQuestion = clearQuestion.replace("где создан", "");
    clearQuestion = clearQuestion.replace("где создали", "");
    clearQuestion = clearQuestion.replace("где производится", "");
    clearQuestion = clearQuestion.replace("где выпускается", "");
    clearQuestion = clearQuestion.replace("чем является", "");
    clearQuestion = clearQuestion.replace("что включает", "");

    clearQuestion = normalize(clearQuestion);

    if (clearQuestion == "") {
        return "Ответ не найден";
    }

    for (var i = 0; i < knowleage.length; i++) {
        var subject = normalize(knowleage[i][0]);
        var predicate = knowleage[i][1].toLowerCase();

        if (subject == clearQuestion) {
            if (neededPredicate == "" || new RegExp(neededPredicate, "i").test(predicate)) {
                return big1(knowleage[i][0]) + " " + knowleage[i][1] + " " + knowleage[i][2] + ".";
            }
        }
    }

    for (var j = 0; j < knowleage.length; j++) {
        var subject2 = normalize(knowleage[j][0]);
        var predicate2 = knowleage[j][1].toLowerCase();

        if (subject2.indexOf(clearQuestion) != -1 || clearQuestion.indexOf(subject2) != -1) {
            if (neededPredicate == "" || new RegExp(neededPredicate, "i").test(predicate2)) {
                return big1(knowleage[j][0]) + " " + knowleage[j][1] + " " + knowleage[j][2] + ".";
            }
        }
    }

    return "Ответ не найден";
}

function dialog_window() {
    $("body").append("<div class='dialog' id='dialog'></div>");
    $("#dialog").append("<div class='dialog_label' onclick='openDialog()'>Диалог</div>");
    $("#dialog").append("<div class='dialog_header'>База знаний</div>");
    $("#dialog").append("<div class='dialog_messages' id='dialog_messages'></div>");
    $("#dialog").append("<div class='dialog_form'><input id='question' placeholder='Введите вопрос' onkeydown='if(event.key === \"Enter\") ask()'><button type='button' onclick='ask()'>Спросить</button><button type='button' id='microphone' onclick='speech()'>🎤</button></div>");
}

let dialogOpen = false;

function openDialog() {
    let dialog = document.getElementById("dialog");
    let label = document.querySelector(".dialog_label");

    if (dialogOpen) {
        if (window.innerWidth <= 480) {
            dialog.style.right = "-100%";
        } else {
            dialog.style.right = "-520px";
            label.style.left = "-78px";
        }

        dialogOpen = false;
    } else {
        if (window.innerWidth <= 480) {
            dialog.style.right = "0";
        } else {
            dialog.style.right = "20px";
            label.style.left = "-62px";
        }

        dialogOpen = true;
    }
}

function ask() {
    var question = $("#question").val();

    if (question == "") {
        return;
    }

    var answer = getAnswer(question);

    $("#dialog_messages").append("<div class='question'>" + question + "</div>");

    var answerBlock = $("<div class='answer'></div>");
    var answerText = $("<span class='answer_text'></span>");
    var speakButton = $("<button type='button' class='speak_button'>🔊</button>");

    answerText.html(answer);

    speakButton.click(function() {
        speak(answerText.html());
    });

    answerBlock.append(answerText);
    answerBlock.append(speakButton);

    $("#dialog_messages").append(answerBlock);

    $("#dialog_messages").scrollTop($("#dialog_messages")[0].scrollHeight);
    $("#question").val("");
}

function speak(text) {
    text = text.replace(/<[^>]*>/g, "");

    if ("speechSynthesis" in window) {
        var utterance = new SpeechSynthesisUtterance(text);
        utterance.lang = "ru-RU";
        window.speechSynthesis.speak(utterance);
    }
}

function speech() {
    var microphone = $("#microphone");

    if (speechOn == true) {
        speechOn = false;
        microphone.removeClass("microphone_active");
        $("#question").attr("placeholder", "Введите вопрос");

        if (recognition != null) {
            recognition.stop();
        }

        return;
    }

    var SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;

    if (!SpeechRecognition) {
        alert("Распознавание речи не поддерживается");
        return;
    }

    recognition = new SpeechRecognition();
    recognition.lang = "ru-RU";
    recognition.interimResults = true;
    recognition.maxAlternatives = 1;
    recognition.continuous = true;

    speechOn = true;
    microphone.addClass("microphone_active");
    $("#question").val("");
    $("#question").attr("placeholder", "Говорите...");

    recognition.start();

    recognition.onresult = function(event) {
        var text = "";

        for (var i = 0; i < event.results.length; i++) {
            text += event.results[i][0].transcript;
        }

        $("#question").val(text);
    };

    recognition.onerror = function(event) {
        $("#question").attr("placeholder", "Ошибка распознавания, попробуйте ещё раз");
    };

    recognition.onend = function() {
        if (speechOn == true) {
            try {
                recognition.start();
            } catch (e) {
                $("#question").attr("placeholder", "Говорите...");
            }
        }
    };
}