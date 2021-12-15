#include <fstream>
#include <iterator>
#include <iostream>
#include <list>
#include <map>
#include <regex>
#include <string>
#include <unordered_set>

int main() {
    int iterations = 40;

    std::fstream input("day14.input");
    std::map<std::string, std::unordered_set<std::string>> rules;
    std::map<std::string, std::int64_t> map_count;
    std::map<std::string, std::int64_t> map_count_new;
    std::map<char, std::int64_t> character_count;

    std::string initial;
    std::getline(input, initial);

    char start = initial[0];
    char end = initial[initial.length()-1];

    std::string line;
    while (std::getline(input, line)) {
        std::regex word_regex("^(\\w\\w)\\s->\\s(\\w)$");
        std::smatch matches;

        if(std::regex_search(line, matches, word_regex)) {
            rules[matches[1].str()].insert(matches[1].str()[0] + matches[2].str());
            rules[matches[1].str()].insert(matches[2].str() + matches[1].str()[1]);
            map_count_new[matches[1].str()] = 0;
            character_count[matches[1].str()[0]] = 0;
        }
    }    

    for (int i = 0; i < initial.length()-1; ++i) {
        std::string key = initial.substr(i, 2);
        ++map_count_new[key];
    }

    for (int i = 0; i < iterations; ++i) {
        map_count = map_count_new;
        for (const auto& [key, amount] : map_count_new) {
            map_count_new[key] = 0;
        }
        for (const auto& [key, amount] : map_count) {
            for (const auto& rule : rules[key]) {
                map_count_new[rule] += amount;
            }
        }
    }

    for (const auto& [key, amount] : map_count_new) {
        character_count[key[0]] += amount;
        character_count[key[1]] += amount;
    }

    character_count[start] += 1;
    character_count[end] += 1;

    std::int64_t min = INT64_MAX;
    std::int64_t max = INT64_MIN;
    for (const auto& [key, amount] : character_count) {
        character_count[key] /= 2;
        min = std::min(min, character_count[key]);
        max = std::max(max, character_count[key]);
        std::cout << key << "=" << character_count[key] << std::endl;
    }
    std::cout << "MIN=" << min << ", MAX=" << max << ", DIFF=" << max-min << std::endl;
}
