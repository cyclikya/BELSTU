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
    question = small1(question);
    question = question.replace("?", " ");
    question = question.replace(".", " ");
    question = question.replace(",", " ");

    var words = question.split(" ");
    var result = false;
    var answer = "";

    for (var i = 0; i < words.length; i++) {
        var predicate = getEnding(words[i]);

        if (predicate != "") {
            var predicateReg = new RegExp(predicate, "i");
            var subject = question.replace(words[i], "");
            subject = subject.replace("что", "");
            subject = subject.replace("кто", "");
            subject = subject.replace("где", "");
            subject = subject.replace("как", "");
            subject = subject.replace("какой", "");
            subject = subject.replace("какая", "");
            subject = subject.replace("какие", "");
            subject = subject.replace("какое", "");
            subject = subject.trim();
            subject = subject.replaceAll(" ", ".*");

            var subjectReg = new RegExp(subject, "i");

            for (var j = 0; j < knowleage.length; j++) {
                if (predicateReg.test(knowleage[j][1]) && 
                    (subjectReg.test(knowleage[j][0]) || subjectReg.test(knowleage[j][2]))) {
                    answer = big1(knowleage[j][0]) + " " + knowleage[j][1] + " " + knowleage[j][2] + ".";
                    result = true;
                    break;
                }
            }
        }

        if (result == true) {
            break;
        }
    }

    if (result == false) {
        var subjectOnly = question;
        subjectOnly = subjectOnly.replace("что такое", "");
        subjectOnly = subjectOnly.replace("кто такой", "");
        subjectOnly = subjectOnly.replace("что", "");
        subjectOnly = subjectOnly.replace("кто", "");
        subjectOnly = subjectOnly.replace("где", "");
        subjectOnly = subjectOnly.replace("как", "");
        subjectOnly = subjectOnly.replace("какой", "");
        subjectOnly = subjectOnly.replace("какая", "");
        subjectOnly = subjectOnly.replace("какие", "");
        subjectOnly = subjectOnly.replace("какое", "");
        subjectOnly = subjectOnly.trim();
        subjectOnly = subjectOnly.replaceAll(" ", ".*");

        var subjectOnlyReg = new RegExp(subjectOnly, "i");

        for (var k = 0; k < knowleage.length; k++) {
            if (subjectOnlyReg.test(knowleage[k][0]) || subjectOnlyReg.test(knowleage[k][2])) {
                answer = big1(knowleage[k][0]) + " " + knowleage[k][1] + " " + knowleage[k][2] + ".";
                result = true;
                break;
            }
        }
    }

    if (result == true) {
        return answer;
    } else {
        return "Ответ не найден";
    }
}

function dialog_window() {
    $("body").append("<div class='dialog' id='dialog'></div>");
    $("#dialog").append("<div class='dialog_label' onclick='openDialog()'>Диалог</div>");
    $("#dialog").append("<div class='dialog_header'>База знаний</div>");
    $("#dialog").append("<div class='dialog_messages' id='dialog_messages'></div>");
    $("#dialog").append("<div class='dialog_form'><input id='question' placeholder='Введите вопрос'><button type='button' onclick='ask()'>Спросить</button><button type='button' id='microphone' onclick='speech()'>🎤</button></div>");
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