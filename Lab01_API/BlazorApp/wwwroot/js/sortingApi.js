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

// Canvas visualization functions
window.initializeCanvas = function () {
    const canvas = document.getElementById('sortingCanvas');
    if (!canvas) {
        throw new Error('Canvas element not found');
    }

    const ctx = canvas.getContext('2d');
    if (!ctx) {
        throw new Error('Failed to get 2D context from canvas');
    }

    // Draw light gray background rectangle
    ctx.fillStyle = '#E8E8E8';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Draw border
    ctx.strokeStyle = '#B0B0B0';
    ctx.lineWidth = 2;
    ctx.strokeRect(0, 0, canvas.width, canvas.height);

    // Add center text
    ctx.fillStyle = '#666666';
    ctx.font = '16px Arial';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText('Canvas initialized successfully!', canvas.width / 2, canvas.height / 2);
};

window.drawSortingArray = function (config) {
    const canvas = document.getElementById('sortingCanvas');
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const {
        array,
        compareIndices,
        sortedIndices,
        pivotIndex,
        canvasWidth,
        canvasHeight,
        barPadding,
        colorUnsorted,
        colorComparing,
        colorSorted,
        colorPivot,
        colorBackground
    } = config;

    // Clear canvas
    ctx.fillStyle = colorBackground;
    ctx.fillRect(0, 0, canvasWidth, canvasHeight);

    if (!array || array.length === 0) return;

    const barWidth = (canvasWidth - (barPadding * array.length)) / array.length;
    const maxValue = Math.max(...array);
    const compareSet = new Set(compareIndices);
    const sortedSet = new Set(sortedIndices);

    for (let i = 0; i < array.length; i++) {
        const value = array[i];
        const barHeight = (value / maxValue) * (canvasHeight - 20);
        const x = i * (barWidth + barPadding);
        const y = canvasHeight - barHeight;

        // Determine color
        let color = colorUnsorted;
        if (i === pivotIndex) {
            color = colorPivot;
        } else if (sortedSet.has(i)) {
            color = colorSorted;
        } else if (compareSet.has(i)) {
            color = colorComparing;
        }

        // Draw bar
        ctx.fillStyle = color;
        ctx.fillRect(x, y, barWidth, barHeight);

        // Draw border around bar
        ctx.strokeStyle = 'rgba(0, 0, 0, 0.2)';
        ctx.lineWidth = 1;
        ctx.strokeRect(x, y, barWidth, barHeight);
    }
};
