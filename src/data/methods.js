// Función específica para GET con manejo de errores
async function getData(url, headers = {}) {
    try {
        const response = await fetch(url, {
            method: 'GET',
            mode: "cors",
            headers: {
                "Content-Type": "application/json",
                "Cache-Control": "no-cache, no-store, must-revalidate",
                ...headers
            },
            credentials: 'omit'
        });

        if (!response.ok) {
            let errorMessage = `Error ${response.status}: ${response.statusText}`;

            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorMessage;
            } catch {
                errorMessage = await response.text();
            }

            throw new Error(errorMessage);
        }

        if (response.status === 204) {
            return null;
        }

        return await response.json();
    } catch (error) {
        throw new Error(error.message);
    }
}

// Función específica para DELETE con manejo de errores
async function deleteData(url, headers = {}) {
    try {
        const response = await fetch(url, {
            method: 'DELETE',
            mode: "cors",
            headers: {
                "Content-Type": "application/json",
                "Cache-Control": "no-cache, no-store, must-revalidate",
                ...headers
            },
        });

        if (!response.ok) {
            let errorMessage = `Error ${response.status}: ${response.statusText}`;

            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorMessage;
            } catch {
                errorMessage = await response.text();
            }

            throw new Error(errorMessage);
        }

        if (response.status === 204) {
            return null;
        }

        return await response.json();
    } catch (error) {
        throw new Error(error.message);
    }
}

// Función específica para POST con manejo de errores
async function postData(url, data, headers = {}) {
    try {
        const response = await fetch(url, {
            method: 'POST',
            mode: "cors",
            headers: {
                "Content-Type": "application/json",
                "Cache-Control": "no-cache, no-store, must-revalidate",
                ...headers
            },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            let errorMessage = "Error en la petición.";

            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorMessage;
            } catch {
                errorMessage = await response.text();
            }

            throw new Error(errorMessage);
        }

        return await response.json();
    } catch (error) {
        throw new Error(error.message);
    }
}

// Función específica para PUT con manejo de errores
async function putData(url, data, headers = {}) {
    try {
        const response = await fetch(url, {
            method: 'PUT',
            mode: "cors",
            headers: {
                "Content-Type": "application/json",
                "Cache-Control": "no-cache, no-store, must-revalidate",
                ...headers
            },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            let errorMessage = "Error en la petición.";

            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorMessage;
            } catch {
                errorMessage = await response.text();
            }

            throw new Error(errorMessage);
        }

        return await response.json();
    } catch (error) {
        throw new Error(error.message);
    }
}

export { getData, deleteData, postData, putData };