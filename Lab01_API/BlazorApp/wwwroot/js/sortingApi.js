window.sortingApi = {
    health: async function (apiBaseUrl) {
        const response = await fetch(`${apiBaseUrl}/api/sorting/health`, {
            method: 'GET'
        });

        if (!response.ok) {
            throw new Error(`Health check failed: ${response.status}`);
        }

        return await response.json();
    },

    algorithms: async function (apiBaseUrl) {
        const response = await fetch(`${apiBaseUrl}/api/sorting/algorithms`, {
            method: 'GET'
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch algorithms: ${response.status}`);
        }

        return await response.json();
    },

    benchmark: async function (apiBaseUrl) {
        const response = await fetch(`${apiBaseUrl}/api/sorting/benchmark`, {
            method: 'GET'
        });

        if (!response.ok) {
            throw new Error(`Benchmark request failed: ${response.status}`);
        }

        return await response.json();
    },

    sort: async function (apiBaseUrl, numbers, algorithm) {
        const response = await fetch(`${apiBaseUrl}/api/sorting/sort`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                numbers: numbers,
                algorithm: algorithm
            })
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || `Sort request failed: ${response.status}`);
        }

        return await response.json();
    },

    quickSort: async function (apiBaseUrl, csvNumbers) {
        const numbers = csvNumbers
            .split(',')
            .map(x => x.trim())
            .filter(x => x.length > 0)
            .map(Number);

        if (numbers.length === 0) {
            return [];
        }

        if (numbers.some(Number.isNaN)) {
            throw new Error('Please enter valid integers separated by commas.');
        }

        const response = await fetch(`${apiBaseUrl}/api/sorting/quicksort`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(numbers)
        });

        if (!response.ok) {
            throw new Error(`Request failed: ${response.status}`);
        }

        return await response.json();
    }
};
