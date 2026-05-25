var dialogOn = false;

var endings =
[
  ["ет", "(ет|ут|ют)"],
  ["ит", "(ит|ат|ят)"],
  ["ает", "(ает|ают)"],
  ["яет", "(яет|яют)"],
  ["ывает", "(ывает|ывают)"],
  ["ивает", "(ивает|ивают)"],
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
  ["покажи", "(показывает|показан|показана|показано|показаны)"],
  ["покажите", "(показывает|показан|показана|показано|показаны)"],
  ["содержит", "(содержит|содержат)"],
  ["поступает", "(поступает|поступают)"],
  ["включает", "(включает|включают)"],
  ["выглядит", "(выглядит|выглядят)"],
  ["выглядят", "(выглядит|выглядят)"],
  ["показан", "(показан|показана|показано|показаны)"],
  ["показано", "(показан|показана|показано|показаны)"],
  ["находится", "располага(ется|ются)"],
  ["находятся", "располага(ется|ются)"],

  ["нужен", "(нужен|нужна|нужно|нужны|предназначен|предназначена|используется|используются|обеспечивает)"],
  ["нужна", "(нужен|нужна|нужно|нужны|предназначен|предназначена|используется|используются|обеспечивает)"],
  ["нужны", "(нужен|нужна|нужно|нужны|предназначен|предназначена|используется|используются|обеспечивает|помогает|освещают)"],

  ["делает", "(работает|создает|обеспечивает|передает|показывает|помогает|освещает|охлаждает|поддерживает|смягчает|повышает|поступает|защищает|содержит|перевозит|движется|управляется|используется|предназначен|нужен|включает|выглядит|показан)"]
];

var questionWords =
[
  "что", "кто", "где", "куда", "когда", "как",
  "какую", "какой", "какая", "какие", "какое",
  "для", "чего", "зачем", "на", "из", "в", "во",
  "это", "такое", "такой", "такая", "такие"
];

function small1(str)
{
  if (str == "") return str;
  return str.substring(0, 1).toLowerCase() + str.substring(1);
}

function big1(str)
{
  if (str == "") return str;
  return str.substring(0, 1).toUpperCase() + str.substring(1);
}

function clearQuestion(question)
{
  question = small1(question);
  question = question.replace(/ё/g, "е");
  question = question.replace(/([?.,!;:])/g, " $1 ");
  question = question.replace(/[?.,!;:]/g, " ");
  question = question.replace(/\s+/g, " ");
  return question.trim();
}

function makeAnswer(i)
{
  return big1(knowleage[i][0]) + " " + knowleage[i][1] + " " + knowleage[i][2] + ".";
}

function getPredicateExpression(word)
{
  var w = word.toLowerCase();

  for (var i = 0; i < endings.length; i++)
  {
    if (w == endings[i][0])
    {
      return endings[i][1];
    }
  }

  for (var j = 0; j < endings.length; j++)
  {
    var pseudo = endings[j][0];
    var variants = endings[j][1];

    if (w.length > pseudo.length && w.substring(w.length - pseudo.length) == pseudo)
    {
      return w.substring(0, w.length - pseudo.length) + variants;
    }
  }

  return "";
}

function getSubjectExpression(words, predicateIndex)
{
  var subjectWords = [];

  for (var i = 0; i < words.length; i++)
  {
    if (i == predicateIndex) continue;
    if (questionWords.indexOf(words[i]) >= 0) continue;
    subjectWords.push(words[i]);
  }

  return subjectWords.join(".*");
}

function findBySubject(subject)
{
  subject = subject.trim().toLowerCase();
  if (subject == "") return "";

  var subjectExpression = subject.split(/\s+/).join(".*");
  var subjectReg = new RegExp(subjectExpression, "i");

  for (var i = 0; i < knowleage.length; i++)
  {
    if (subjectReg.test(knowleage[i][0].toLowerCase()))
    {
      return makeAnswer(i);
    }
  }

  for (var j = 0; j < knowleage.length; j++)
  {
    if (subjectReg.test(knowleage[j][2].toLowerCase()))
    {
      return makeAnswer(j);
    }
  }

  return "";
}

function findBySubjectAndPredicate(subjectExpression, predicateExpression)
{
  if (subjectExpression == "" || predicateExpression == "") return "";

  var subjectReg = new RegExp(subjectExpression, "i");
  var predicateReg = new RegExp(predicateExpression, "i");

  for (var i = 0; i < knowleage.length; i++)
  {
    if (predicateReg.test(knowleage[i][1].toLowerCase()) &&
        subjectReg.test(knowleage[i][0].toLowerCase()))
    {
      return makeAnswer(i);
    }
  }

  for (var j = 0; j < knowleage.length; j++)
  {
    if (predicateReg.test(knowleage[j][1].toLowerCase()) &&
        subjectReg.test(knowleage[j][2].toLowerCase()))
    {
      return makeAnswer(j);
    }
  }

  return "";
}

function getAnswer(question)
{
  var q = clearQuestion(question);
  var answer = "";

  if (q == "") return "Введите вопрос.";

  if (q.indexOf("что такое ") == 0)
  {
    answer = findBySubject(q.replace("что такое ", ""));
    if (answer != "") return answer;
  }

  if (q.indexOf("кто такой ") == 0)
  {
    answer = findBySubject(q.replace("кто такой ", ""));
    if (answer != "") return answer;
  }

  var words = q.split(" ");

  for (var i = 0; i < words.length; i++)
  {
    var predicateExpression = getPredicateExpression(words[i]);

    if (predicateExpression != "")
    {
      var subjectExpression = getSubjectExpression(words, i);
      answer = findBySubjectAndPredicate(subjectExpression, predicateExpression);

      if (answer != "") return answer;
    }
  }

  answer = findBySubject(q);
  if (answer != "") return answer;

  return "Ответ не найден";
}

function dialog_window()
{
  var dialog = document.createElement("div");
  dialog.id = "dialog";
  dialog.className = "dialog";

  var label = document.createElement("div");
  label.className = "dialog_label";
  label.innerHTML = "Диалог";
  label.onclick = openDialog;

  var header = document.createElement("div");
  header.className = "dialog_header";
  header.innerHTML = "Диалог с базой знаний";
  header.onclick = openDialog;

  var messages = document.createElement("div");
  messages.id = "dialog_messages";
  messages.className = "dialog_messages";

  var inputBlock = document.createElement("div");
  inputBlock.className = "dialog_input_block";

  var input = document.createElement("input");
  input.id = "dialog_input";
  input.className = "dialog_input";
  input.placeholder = "Введите вопрос";

  var button = document.createElement("button");
  button.className = "dialog_button";
  button.innerHTML = "Спросить";
  button.onclick = ask;

  input.onkeydown = function(event)
  {
    if (event.keyCode == 13)
    {
      ask();
    }
  };

  inputBlock.appendChild(input);
  inputBlock.appendChild(button);

  dialog.appendChild(label);
  dialog.appendChild(header);
  dialog.appendChild(messages);
  dialog.appendChild(inputBlock);

  document.body.appendChild(dialog);
}

function openDialog()
{
  if (window.innerWidth <= 480)
  {
    if (dialogOn == false)
    {
      $("#dialog").animate({right: "0"}, 500);
      dialogOn = true;
    }
    else
    {
      $("#dialog").animate({right: "-100%"}, 500);
      dialogOn = false;
    }
  }
  else
  {
    if (dialogOn == false)
    {
      $("#dialog").animate({right: "20px"}, 500);
      dialogOn = true;
    }
    else
    {
      $("#dialog").animate({right: "-360px"}, 500);
      dialogOn = false;
    }
  }
}

function ask()
{
  var input = document.getElementById("dialog_input");
  var question = input.value;

  if (question.trim() == "")
  {
    return;
  }

  if (dialogOn == false)
  {
    openDialog();
  }

  var messages = document.getElementById("dialog_messages");

  var userMessage = document.createElement("div");
  userMessage.className = "question";
  userMessage.innerHTML = question;
  messages.appendChild(userMessage);

  var answerMessage = document.createElement("div");
  answerMessage.className = "answer";
  answerMessage.innerHTML = getAnswer(question);
  messages.appendChild(answerMessage);

  messages.scrollTop = messages.scrollHeight;
  input.value = "";
  input.focus();
}