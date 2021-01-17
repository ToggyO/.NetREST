let token;
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/api/ws/chat", { accessTokenFactory: () => token})
    .build();

hubConnection.on("Receive", function (message, userName) {

    let userNameElem = document.createElement("b");
    userNameElem.appendChild(document.createTextNode(userName + ": "));

    let elem = document.createElement("p");
    elem.appendChild(userNameElem);
    elem.appendChild(document.createTextNode(message));

    const firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(elem, firstElem);
});

hubConnection.on("Notify", function (message) {

    let elem = document.createElement("p");
    elem.appendChild(document.createTextNode(message));

    const firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(elem, firstElem);
});

hubConnection.on('GetGroupsList', function (groupsList) {
    const groupsContainer = document.querySelector('.groups-list');
    groupsContainer.innerHTML = '';
    const list = document.createElement('ul');
    list.classList.add('groupsList');
    
    for (let i = 0; i < groupsList.length; i++) {
       const li = document.createElement('li');
       li.classList.add('group-list-item');
       li.appendChild(document.createTextNode(groupsList[i].name));
       list.appendChild(li);
    }

    groupsContainer.appendChild(list);
});

document.getElementById('loginBtn').addEventListener('click', function () {
    let request = new XMLHttpRequest();
    request.open('POST', 'http://localhost:5000/api/auth/token', true);
    request.setRequestHeader('Content-Type', 'application/json');
    request.addEventListener('load', function () {
        if (request.status < 400) {
            let data = JSON.parse(request.response);
            token = data.resultData.token.accessToken;
            document.getElementById("sendBtn").disabled = false;

            hubConnection.start()
                .then(() => getGroupsList())
                .catch(err => {
                    console.error(err.toString());
                    document.getElementById("loginBtn").disabled = true;
                    document.getElementById("sendBtn").disabled = true;
                });
        } else {
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

document.getElementById("sendBtn").addEventListener("click", function (e) {
    let message = document.getElementById("message").value;
    let to = document.getElementById("receiver").value;
    hubConnection.invoke("SendMessage", message, to);
});


document.getElementById('createGroupBtn').addEventListener('click', function () {
    if (!token) {
        return;
    }
    const groupName = document.getElementById('create-group-input').value;
    console.log(groupName);
    createGroup(groupName);
});

function getGroupsList() {
    if (!token) {
        return;
    }
    hubConnection.invoke('GetGroupsList');
}

function createGroup(groupName) {
    hubConnection.invoke('CreateGroup', groupName)
        .then(() => getGroupsList());
}

