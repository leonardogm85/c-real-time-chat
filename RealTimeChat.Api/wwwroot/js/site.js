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

const ready = () => {
    const register = document.getElementById('register');
    const logIn = document.getElementById('logIn');

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
            .then(() => console.info('Registered Successfully!'))
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
            .then(() => console.info('Logged In Successfully!'))
            .catch(console.error);
    });

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
};
