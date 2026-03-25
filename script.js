let totalAmount = 0;
let categoryTotals = {}; 

document.getElementById('add-btn').addEventListener('click', function() {
    const descInput = document.getElementById('desc');
    const amountInput = document.getElementById('amount');
    const categoryInput = document.getElementById('category');

    const desc = descInput.value;
    const amount = parseFloat(amountInput.value);
    const category = categoryInput.value;

    if (desc === "" || isNaN(amount) || amount <= 0) {
        alert("Ole hyvä ja syötä tiedot!");
        return;
    }

    // päivitä taulukko
    const tableBody = document.getElementById('expense-body');
    const newRow = document.createElement('tr');
    const date = new Date().toLocaleDateString('fi-FI');

    newRow.innerHTML = `
        <td>${date}</td>
        <td>${desc}</td>
        <td>${category}</td>
        <td>${amount.toFixed(2)} €</td>
        <td><button class="delete-btn" onclick="deleteRow(this, '${category}', ${amount})">Poista</button></td>
    `;
    tableBody.appendChild(newRow);

    updateFinancials(category, amount);

    // tyhjennä kentät
    descInput.value = "";
    amountInput.value = "";
});

function updateFinancials(category, amount) {
    // Päivitä kokonaissumma
    totalAmount += amount;
    document.getElementById('total-amount').innerText = `${totalAmount.toFixed(2)} €`;

    // Päivitä kategoriakohtainen summa
    if (!categoryTotals[category]) {
        categoryTotals[category] = 0;
    }
    categoryTotals[category] += amount;

    // Etsi suurin kategoria
    updateTopCategory();
}

function updateTopCategory() {
    let topCat = "-";
    let maxSafeAmount = 0;

    for (const cat in categoryTotals) {
        if (categoryTotals[cat] > maxSafeAmount) {
            maxSafeAmount = categoryTotals[cat];
            topCat = cat;
        }
    }

    // päivitetään UI:ssä näkyvä kategoria ja summa
    document.getElementById('top-category').innerText = maxSafeAmount > 0 
        ? `${topCat} (${maxSafeAmount.toFixed(2)} €)` 
        : "-";
}

function deleteRow(btn, category, amount) {
    const row = btn.parentNode.parentNode;
    row.parentNode.removeChild(row);
    
    // Vähennetään summat
    totalAmount -= amount;
    categoryTotals[category] -= amount;
    
    // UI
    document.getElementById('total-amount').innerText = `${totalAmount.toFixed(2)} €`;
    updateTopCategory();
}