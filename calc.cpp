#include <cmath>
#include <iostream>

int main() {
    double a, b, result = 0;
    int op;
    std::cout << "First number: ";
    if (!(std::cin >> a)) return 1;
    std::cout << "Operation (1=+, 2=-, 3=*, 4=/): ";
    if (!(std::cin >> op)) return 1;
    std::cout << "Second number: ";
    if (!(std::cin >> b)) return 1;
    switch (op) {
        case 1:
            result = a + b;
            break;
        case 2:
            result = a - b;
            break;
        case 3:
            result = a * b;
            break;
        case 4:
            if (b == 0) { std::cerr << "Division by zero\n"; return 1; }
            result = a / b;
            break;
        default:
            std::cerr << "Unknown operation\n";
            return 1;
    }
    if (!std::isfinite(result)) { std::cerr << "Invalid result\n"; return 1; }
    std::cout << "Result = " << result << '\n';
}
