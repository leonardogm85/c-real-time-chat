const connection = new signalR
    .HubConnectionBuilder()
    .withUrl("/ChatHub")
    .build();

const startConnection = async () => {
    await connection
        .start()
        .then(() => {
            console.info('Started!');
            ready();
        })
        .catch((err) => {
            console.error(err);
            setTimeout(startConnection, 5000);
        });
};

startConnection();

const ready = async () => {
    const register = document.getElementById('register');
    const logIn = document.getElementById('logIn');
    const logOut = document.getElementById('logOut');
    const redirectRegister = document.getElementById('goRegister');
    const redirectLogIn = document.getElementById('backLogIn');

    const talk = document.getElementById('talk');

    if (logIn && getLoggedInUser()) {
        goTalk();
    }

    if (talk) {
        if (getLoggedInUser()) {
            await connection
                .invoke('AddConnection', getLoggedInUser().id)
                .then(() => console.info('AddConnection Invoked Successfully!'))
                .catch(console.error);
        } else {
            goLogIn();
        }
    }

    register?.addEventListener('click', async () => {
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
    });

    logIn?.addEventListener('click', async () => {
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
    });

    logOut?.addEventListener('click', async () => {
        await connection
            .invoke('RemoveConnection', getLoggedInUser().id)
            .then(() => {
                removeLoggedInUser();
                goLogIn();
            })
            .catch(console.error);
    });

    redirectRegister?.addEventListener('click', goRegister);

    redirectLogIn?.addEventListener('click', goLogIn);

    connection.onclose(startConnection);

    connection.on('ReceiveRegisteredUser', (success, user, message) => {
        const result = document.getElementById('result');

        if (success) {
            document.getElementById('name').value = '';
            document.getElementById('email').value = '';
            document.getElementById('password').value = '';
            console.info(user);
        }

        result.innerText = message;
    });

    connection.on('ReceiveLoggedInUser', (success, user, message) => {
        if (success) {
            setLoggedInUser(user);
            goTalk();
            console.info(user);
        } else {
            document.getElementById('result').innerText = message;
        }
    });
};

const setLoggedInUser = (loggedInUser) => {
    sessionStorage.setItem('loggedInUser', JSON.stringify(loggedInUser));
};

const getLoggedInUser = () => {
    return JSON.parse(sessionStorage.getItem('loggedInUser'));
};

const removeLoggedInUser = () => {
    sessionStorage.removeItem('loggedInUser');
};

const goLogIn = () => window.location = '/Home/LogIn';

const goRegister = () => window.location = '/Home/Register';

const goTalk = () => window.location = '/Home/Talk';
