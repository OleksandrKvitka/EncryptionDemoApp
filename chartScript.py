import matplotlib.pyplot as plt
import csv

# Function to read data from a CSV file
def read_csv_data(file_path):
    with open(file_path, 'r') as file:
        reader = csv.reader(file)
        headers = next(reader)  # Read the first row as headers
        values = next(reader)   # Read the second row as values
        return headers, list(map(float, values))

# Main script
file_path = "/Users/okvitka/Documents/Aspirantura/Дисертація/Code/EncryptionDemoApp/data/multiplication_performance_results.csv"
categories, values = read_csv_data(file_path)

# Create bar chart
plt.figure(figsize=(12, 6))
plt.bar(categories, values, edgecolor="black")

# Add labels and title
plt.xlabel("Multiplier Types", fontsize=12)
plt.ylabel("Time, ms", fontsize=12)
plt.title("Multiplier Performance Comparison", fontsize=14)
plt.xticks(rotation=45, ha="right", fontsize=10)

# Add value annotations on bars
for i, v in enumerate(values):
    plt.text(i, v + (0.5 if v >= 0 else -0.5), str(v), ha="center", va="bottom" if v >= 0 else "top")

# Show grid for better readability
plt.grid(axis="y", linestyle="--", alpha=0.7)

# Display the chart
plt.tight_layout()
plt.show()
