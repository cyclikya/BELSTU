var dialogOn = false;

var endings = [
    ["ет", "(ет|ут|ют)"], 
    ["ут", "(ет|ут|ют)"], 
    ["ют", "(ет|ут|ют)"],
    ["ит", "(ит|ат|ят)"], 
    ["ат", "(ит|ат|ят)"], 
    ["ят", "(ит|ат|ят)"],
    ["ется", "(ет|ут|ют)ся"], 
    ["утся", "(ет|ут|ют)ся"], 
    ["ются", "(ет|ут|ют)ся"],
    ["ится", "(ит|ат|ят)ся"], 
    ["атся", "(ит|ат|ят)ся"], 
    ["ятся", "(ит|ат|ят)ся"],
    ["ен", "ен"], 
    ["ена", "ена"], 
    ["ено", "ено"], 
    ["ены", "ены"],
    ["ан", "ан"], 
    ["ана", "ана"], 
    ["ано", "ано"], 
    ["аны", "аны"],
    ["жен", "жен"], 
    ["жна", "жна"], 
    ["жно", "жно"], 
    ["жны", "жны"]
];

function getEnding(word) {
    if (!word) return -1;
    for (var j = 0; j < endings.length; j++) {
        if (word.substring(word.length - endings[j][0].length) == endings[j][0]) {
            return j;
        }
    }
    return -1;
}

function small(str) {
    if (!str) return "";
    return str.substring(0, 1).toLowerCase() + str.substring(1);
}

function big(str) {
    if (!str) return "";
    return str.substring(0, 1).toUpperCase() + str.substring(1);
}

function clearQuestion(question) {
    return small(question)
        .replace(/[?!.;,:\-–—]/g, " ")
        .replace(/\s+/g, " ")
        .trim();
}

function getAnswer(question) {
    var result = false;
    var answer = "";
    var base = knowledge;
    var groups = {};

    question = clearQuestion(question || "");
    question = question.replace("что такое ", "чем является ");
    question = question.replace("покажи мне ", "что показывает ");
    question = question.replace("покажи ", "что показывает ");

    if (question.length == 0) return "Введите вопрос. <br/>";

    var words = question.split(" ");

    function add(row) {
        var key = row[0].toLowerCase() + "||" + row[1].toLowerCase();

        if (!groups[key]) {
            groups[key] = [row[0], row[1], []];
        }

        if (groups[key][2].indexOf(row[2]) == -1) {
            groups[key][2].push(row[2]);
        }
    }

    for (var i = 0; i < words.length; i++) {
        var ending = getEnding(words[i]);

        if (ending >= 0) {
            words[i] = words[i].substring(0, words[i].length - endings[ending][0].length) + endings[ending][1];

            var predicate = new RegExp(words[i], "i");

            var subject_string = words.slice(i + 1).join(".*");

            if (subject_string.length > 3) {
                var subject = new RegExp(".*" + subject_string + ".*", "i");

                for (var j = 0; j < base.length; j++) {
                    if (predicate.test(base[j][1]) && (subject.test(base[j][0]) || subject.test(base[j][2]))) {
                        add(base[j]);
                        result = true;
                    }
                }

                if (!result) {
                    for (var k = 0; k < base.length; k++) {
                        if (subject.test(base[k][0]) || subject.test(base[k][2])) {
                            add(base[k]);
                            result = true;
                        }
                    }
                }
            }
        }
    }

    if (!result) return "Ответ не найден. <br/>";

    for (var key in groups) {
        answer += big(groups[key][0] + " " + groups[key][1] + " " + groups[key][2].join(", ")) + ". ";
    }

    return answer;
}

function dialog_window() {
    if (document.getElementById("dialog")) return;

    document.body.insertAdjacentHTML("beforeend",
        "<div id='dialog' class='dialog'>" +
            "<div class='dialog_label label' onclick='openDialog()'>Спросить</div>" +
            "<div class='dialog_header header' onclick='openDialog()'>Диалог с базой знаний</div>" +
            "<div class='dialog_messages history' id='history'></div>" +
            "<div class='dialog_input_block'>" +
                "<input id='Qdialog' class='dialog_input' placeholder='Введите вопрос'/>" +
                "<button class='dialog_button' onclick='ask(\"Qdialog\")'>Спросить</button>" +
                "<button class='dialog_button voice_button' type='button' onclick='startBrowserSpeech()'>🎤</button>" +
            "</div>" +
        "</div>"
    );

    document.getElementById("Qdialog").onkeydown = function(event) {
        if (event.key == "Enter") ask("Qdialog");
    };

    if (window.ya && ya.speechkit && ya.speechkit.Textline) {
        try {
            ya.speechkit.settings.apikey = "5c6d6536-b453-4589-9bc7-f16c7a795106";
            new ya.speechkit.Textline("Qdialog", {
                onInputFinished: function(text) {
                    document.getElementById("Qdialog").value = text;
                }
                });
        } catch (e) {}
    }
}

function getClosedRight() {
    if (window.innerWidth <= 480) return "-100%";
    return "-520px";
}

function getOpenedRight() {
    if (window.innerWidth <= 480) return "0px";
    return "30px";
}

function moveDialog(rightValue, speed) {
    var dialog = document.getElementById("dialog");
    if (!dialog) return;

    if (window.jQuery) {
        $("#dialog").stop(true, true).animate({ "right": rightValue }, speed, function() {});
    } else {
        dialog.style.right = rightValue;
    }
}

function openDialog() {
    if (dialogOn) {
        moveDialog(getClosedRight(), 1000);
        dialogOn = false;
    } else {
        moveDialog(getOpenedRight(), 1000);
        dialogOn = true;
        if (typeof timer != "undefined") clearInterval(timer);
    }
}

function ask(questionInput) {
    var input = document.getElementById(questionInput);
    if (!input) return;

    var question = input.value;
    if (question.replace(/\s+/g, "").length == 0) return;

    dialogOn = true;
    moveDialog(getOpenedRight(), 300);

    var newDiv = document.createElement("div");
    newDiv.className = "question";
    newDiv.innerHTML = question;
    document.getElementById("history").appendChild(newDiv);

    newDiv = document.createElement("div");
    newDiv.className = "answer";

    var answer = getAnswer(question);
    newDiv.innerHTML = answer;

    var speakButton = document.createElement("button");
    speakButton.className = "speak_button";
    speakButton.innerHTML = "Озвучить";
    speakButton.onclick = function() {
        speakAnswer(answer);
    };
    newDiv.appendChild(speakButton);

    document.getElementById("history").appendChild(newDiv);
    document.getElementById("history").scrollTop = document.getElementById("history").scrollHeight;
    input.value = "";
}

function startBrowserSpeech() {
    var SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    if (!SpeechRecognition) {
        var input = document.getElementById("Qdialog");
        if (input) input.placeholder = "Голосовой ввод недоступен в этом браузере";
        return;
    }

    var recognition = new SpeechRecognition();
    recognition.lang = "ru-RU";
    recognition.interimResults = false;
    recognition.maxAlternatives = 1;

    recognition.onresult = function(event) {
        document.getElementById("Qdialog").value = event.results[0][0].transcript;
    };

    recognition.start();
}

function speakAnswer(answer) {
    if (!window.speechSynthesis) return;

    var text = answer.replace(/<[^>]+>/g, " ").replace(/\s+/g, " ").trim();
    if (text.length == 0) return;

    var utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = "ru-RU";
    window.speechSynthesis.cancel();
    window.speechSynthesis.speak(utterance);
}
