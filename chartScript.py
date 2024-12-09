import pandas as pd
import matplotlib.pyplot as plt

# Load data from three CSV files
data_rsa_aes = pd.read_csv("AesAndRsa_performance.csv")
data_ecdsa = pd.read_csv("ECIESAndAES_performance.csv")
data_ed25519 = pd.read_csv("EdEccAndAes_performance.csv")

# Assuming the structure of the files is the same, and we add a 'Method' column to differentiate them
data_rsa_aes["Method"] = "AES & RSA"
data_ecdsa["Method"] = "AES & Secp256r1"
data_ed25519["Method"] = "AES & Curve25519"

# Concatenate the data into one DataFrame
combined_data = pd.concat([data_rsa_aes, data_ecdsa, data_ed25519])

# Plot data
plt.figure(figsize=(10, 6))

# Plot each method's data
for method in combined_data["Method"].unique():
    method_data = combined_data[combined_data["Method"] == method]
    plt.plot(method_data["Plaintext Size (KB)"], method_data["Total Time (seconds)"], label=method, marker='o')

# Set chart labels and title
plt.xlabel("Розмір вхідного тексту, Кб")
plt.ylabel("Час роботи, с")
plt.legend()
plt.grid(True)

# Show the plot
plt.show()