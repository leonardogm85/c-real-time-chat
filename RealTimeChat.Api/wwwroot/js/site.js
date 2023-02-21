// Connection

const createConnection = () => {
    return new signalR
        .HubConnectionBuilder()
        .withUrl("/ChatHub")
        .build();
};

const startConnection = async () => {
    await connection
        .start()
        .then(() => {
            ready();
            console.info('Started!');
        })
        .catch((err) => {
            if (connection.state === 0) {
                setTimeout(startConnection, 5000);
                console.error(err);
            }
        });
};

// Session storage

const setLoggedInUser = (loggedInUser) => {
    sessionStorage.setItem('loggedInUser', JSON.stringify(loggedInUser));
};

const getLoggedInUser = () => {
    return JSON.parse(sessionStorage.getItem('loggedInUser'));
};

const removeLoggedInUser = () => {
    sessionStorage.removeItem('loggedInUser');
};

// Redirect

const goLogIn = () => {
    window.location = '/Home/LogIn'
};

const goRegister = () => {
    window.location = '/Home/Register'
};

const goTalk = () => {
    window.location = '/Home/Talk'
};

// Events

const receiveLoggedInUser = (success, user, message) => {
    if (success) {
        setLoggedInUser(user);
        goTalk();
    } else {
        document.getElementById('result').innerText = message;
    }
};

const receiveRegisteredUser = (success, _, message) => {
    const result = document.getElementById('result');

    if (success) {
        document.getElementById('name').value = '';
        document.getElementById('email').value = '';
        document.getElementById('password').value = '';
    }

    result.innerText = message;
};

const receiveUsers = (users) => {
    const list = document.getElementById('users');

    const items = list.getElementsByClassName('talk-user-item');

    list.innerHTML = users
        .filter(u => u.id !== getLoggedInUser().id)
        .reduce(loadUsers, '');

    Array
        .from(items)
        .forEach(i => i.addEventListener('click', createGroup));
};

const receiveGroup = (groupName, messages) => {
    document.getElementById('form').classList.remove('display-none');
    document.getElementById('groupName').value = groupName;

    const content = document.getElementById('messages');

    content.innerHTML = messages.reduce(loadMessages, '');
    content.scrollTo(0, content.scrollHeight);
};

const receiveMessage = (message) => {
    if (message.groupName === document.getElementById('groupName').value) {
        const content = document.getElementById('messages');

        content.innerHTML = loadMessages(content.innerHTML, message);
        content.scrollTo(0, content.scrollHeight);
    }
};

// Listener

const registerUser = async () => {
    const name = document.getElementById('name');
    const email = document.getElementById('email');
    const password = document.getElementById('password');

    const user = {
        name: name.value,
        email: email.value,
        password: password.value
    };

    await connection
        .invoke('Register', user)
        .then(() => console.info('Register Invoked Successfully!'))
        .catch(console.error);
};

const logInUser = async () => {
    const email = document.getElementById('email');
    const password = document.getElementById('password');

    const user = {
        email: email.value,
        password: password.value
    };

    await connection
        .invoke('LogIn', user)
        .then(() => console.info('LogIn Invoked Successfully!'))
        .catch(console.error);
};

const logOutUser = async () => {
    await connection
        .invoke('RemoveConnection', getLoggedInUser().id)
        .then(() => {
            removeLoggedInUser();
            goLogIn();
        })
        .catch(console.error);
};

const sendMessage = async () => {
    const groupName = document.getElementById('groupName');
    const textMessage = document.getElementById('textMessage');

    await connection
        .invoke('SendMessage', getLoggedInUser().id, groupName.value, textMessage.value)
        .then(() => console.info('SendMessage Invoked Successfully!'))
        .catch(console.error);

    textMessage.value = '';
};

const createGroup = async (event) => {
    const loggedInUserEmail = getLoggedInUser().email;

    const selectedUserEmail = event.target
        .closest('.talk-user-item')
        .querySelector('.talk-user-email')
        .innerText;

    await connection
        .invoke('CreateGroup', loggedInUserEmail, selectedUserEmail)
        .then(() => console.info('CreateGroup Invoked Successfully!'))
        .catch(console.error);
};

const unload = async () => {
    await connection
        .invoke('RemoveConnection', getLoggedInUser().id)
        .then(() => console.info('RemoveConnection Invoked Successfully!'))
        .catch(console.error);
};

// Load

const loadUsers = (content, user) => {
    return content
        + `<div class="talk-user-item">
                <img class="talk-image-user" src="/images/chat.png" />
                <div>
                    <div class="talk-user-name">${user.name} (${user.isOnline ? 'online' : 'offline'})</div>
                    <div class="talk-user-email">${user.email}</div>
                </div>
            </div>`;
};

const loadMessages = (content, message) => {
    return content
        + `<div class="task-message-item ${message.userId === getLoggedInUser().id ? 'talk-message-right' : 'talk-message-left'}">
                <div class="task-message-head">
                    <img src="/images/message.png" />
                    ${message.userName}
                </div>
                <div class="task-message-body">
                    ${message.text}
                </div>
            </div>`;
};

// Ready

const ready = () => {
    // Close

    connection.onclose(startConnection);

    // LogIn

    const logIn = document.getElementById('logIn');
    const redirectRegister = document.getElementById('goRegister');

    if (logIn && getLoggedInUser()) {
        goTalk();
    }

    connection.on('ReceiveLoggedInUser', receiveLoggedInUser);

    redirectRegister?.addEventListener('click', goRegister);
    logIn?.addEventListener('click', logInUser);

    // Register

    const register = document.getElementById('register');
    const redirectLogIn = document.getElementById('backLogIn');

    connection.on('ReceiveRegisteredUser', receiveRegisteredUser);

    register?.addEventListener('click', registerUser);
    redirectLogIn?.addEventListener('click', goLogIn);

    // Talk

    const logOut = document.getElementById('logOut');
    const send = document.getElementById('send');

    const talk = document.getElementById('talk');

    if (talk) {
        if (getLoggedInUser()) {
            connection
                .invoke('GetUsers')
                .then(() => console.info('GetUsers Invoked Successfully!'))
                .catch(console.error);

            connection
                .invoke('AddConnection', getLoggedInUser().id)
                .then(() => console.info('AddConnection Invoked Successfully!'))
                .catch(console.error);

            addEventListener('unload', unload);
        } else {
            goLogIn();
        }
    }

    connection.on('ReceiveUsers', receiveUsers);
    connection.on('ReceiveGroup', receiveGroup);
    connection.on('ReceiveMessage', receiveMessage);

    logOut?.addEventListener('click', logOutUser);
    send?.addEventListener('click', sendMessage);
};

// Start

const connection = createConnection();

startConnection();
