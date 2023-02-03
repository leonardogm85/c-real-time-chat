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

connection.onclose(startConnection);

const ready = () => {
    const register = document.getElementById('register');

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
            .then(console.error);
    });
};
