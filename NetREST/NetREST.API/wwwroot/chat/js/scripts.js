const $sendBtn = document.getElementById("sendBtn");
const $leaveGroupBtn = document.getElementById("leaveGroupBtn");
const $chatRoom = document.getElementById("chatroom");
const $textMessage = document.getElementById('textMessage');
const $loginBtn = document.getElementById('loginBtn');
const $regBtn = document.getElementById('regBtn');
const $portal = document.getElementById('portal');

let token = null;
let currentGroupId = null;
let currentUser = null;
const getApiUrl = (url) => `http://172.16.1.122:5000/api${url}`;

const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(getApiUrl('/ws/chat'), { accessTokenFactory: () => token})
    .build();

const getGroupsList = () => token && hubConnection.invoke('GetGroupsList');
const createGroup = (groupName) => hubConnection.invoke('CreateGroup', groupName);
const enterGroup = (groupId) => hubConnection.invoke('EnterGroup', groupId)
const leaveGroup = (groupId) => hubConnection.invoke("LeaveGroup", groupId);
const sendMessage = (message) => hubConnection.invoke("SendMessage", message);

const scrollChatToBottom = () => $chatRoom.scrollTop = $chatRoom.scrollHeight;

hubConnection.on("SendMessage", function ({ message, user }) {
    let userNameElem = document.createElement("b");
    userNameElem.appendChild(document.createTextNode(user + ": "));

    let container = document.createElement('div');
    let elem = document.createElement('p');
    elem.classList.add('chat-message');
    if (user === currentUser) {
        elem.classList.add('own-message');
        container.classList.add('own-message__container');
    }
    elem.appendChild(userNameElem);
    elem.appendChild(document.createTextNode(message));
    container.appendChild(elem);
    
    $chatRoom.appendChild(container);
    scrollChatToBottom();
});

hubConnection.on("Notify", function (message) {
    let elem = document.createElement("p");
    elem.appendChild(document.createTextNode(message));
    $chatRoom.appendChild(elem);
    scrollChatToBottom();
});

hubConnection.on('GetGroupsList', function (groupsList) {
    const groupsContainer = document.querySelector('.groups-list');
    groupsContainer.innerHTML = '';
    const list = document.createElement('ul');
    list.classList.add('groupsList');
    
    for (let i = 0; i < groupsList.length; i++) {
       const li = document.createElement('li');
       li.classList.add('group-list-item');
       li.onclick = () => enterGroup(groupsList[i].id).then(() => {
           $leaveGroupBtn.disabled = false;
           $sendBtn.disabled = false;
       });
       li.appendChild(document.createTextNode(groupsList[i].name));
       list.appendChild(li);
    }

    groupsContainer.appendChild(list);
});

hubConnection.on('GetGroupById', (group, message) => currentGroupId = group.id);

hubConnection.on('GroupLeft', function (group) {
    currentGroupId = null;
    $leaveGroupBtn.disabled = true;
    $chatRoom.innerHTML = '';
});

$regBtn.addEventListener('click', function () {
    let request = new XMLHttpRequest();
    request.open('POST', getApiUrl('/auth/signup'), true);
    request.setRequestHeader('Content-Type', 'application/json');
    request.addEventListener('load', function () {
        if (request.status < 400) {
            const popup = popupOpen($portal, 'Успех!', 'Вы создали пользователя =)');
            setTimeout(() => popupClose($portal, popup), 3000);
        } else {
            const popup = popupOpen($portal, 'Ошибка!', 'Идите нахуй', true);
            setTimeout(() => popupClose($portal, popup), 3000);
            console.log("Status", request.status);
            console.log("Response", request.response);
            console.log(request.responseText);
        }
    });
    const regPayload = {
        firstName: document.getElementById('firstName').value,
        email: document.getElementById('regEmail').value,
        password: document.getElementById('regPassword').value,
    };
    request.send(JSON.stringify(regPayload));
});

$loginBtn.addEventListener('click', function () {
    let request = new XMLHttpRequest();
    request.open('POST', getApiUrl('/auth/token'), true);
    request.setRequestHeader('Content-Type', 'application/json');
    request.addEventListener('load', function () {
        if (request.status < 400) {
            let data = JSON.parse(request.response);
            const { token: userToken = {}, user = {} } = data?.resultData;
            token = userToken.accessToken;
            currentUser = user.firstName;
            hubConnection.start()
                .then(() => getGroupsList())
                .catch(err => {
                    console.error(err.toString());
                    document.getElementById("loginBtn").disabled = true;
                    document.getElementById("sendBtn").disabled = true;
                });
        } else {
            const popup = popupOpen($portal, 'Такого пользователя не существует!', 'Идите нахуй', true);
            setTimeout(() => popupClose($portal, popup), 3000);
            console.log("Status", request.status);
            console.log("Response", request.response);
            console.log(request.responseText);
        }
    });
    const loginPayload = {
        email: document.getElementById("userName").value,
        password: document.getElementById("userPassword").value,
    };
    request.send(JSON.stringify(loginPayload));
});

$sendBtn.addEventListener("click", () => {
    const value = $textMessage.value?.trim() || '';
    if (value) {
        sendMessage($textMessage.value).then(() => $textMessage.value = '');
    }
});
$leaveGroupBtn.addEventListener('click', function () {
    if (!token || !currentGroupId)
        return;
    leaveGroup(currentGroupId);
    $sendBtn.disabled = true;
    $leaveGroupBtn.disabled = true;
    $chatRoom.innerHTML = '';
});
document.getElementById('createGroupBtn').addEventListener('click', function () {
    if (!token) {
        return;
    }
    const groupName = document.getElementById('create-group-input').value;
    createGroup(groupName).then(() => getGroupsList());
});
$textMessage.addEventListener('keyup', function (e) {
    e.preventDefault();
    const value = $textMessage.value?.trim() || '';
    if (e.key === 'Enter' && value) {
        sendMessage($textMessage.value).then(() => $textMessage.value = '');
    }
});

function createPopup(header, text) {
    const popup = document.createElement('div');
    const headline = document.createElement('div');
    const content = document.createElement('div');
    popup.id = 'popup';
    popup.classList.add('popup');
    headline.classList.add('popup__headline');
    headline.textContent = header;
    content.textContent = text;
    content.classList.add('popup__content');
    popup.appendChild(headline);
    popup.appendChild(content);
    return popup;
}

function popupOpen($container, headline, content, isError) {
    const popup = createPopup(headline, content);
    if (isError) {
        popup.classList.add('popup__error');
    }
    $container.append(popup);
    return popup;
}

function popupClose($container, popup) {
    popup.classList.add('hide');
    setTimeout(() => {
        $container.removeChild(popup)
    }, 500);
}